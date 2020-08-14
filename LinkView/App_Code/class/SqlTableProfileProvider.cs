/// <summary>
/// Custom SQL Table Profile Provider
/// </summary>

#region Custom Table Profile Provider

namespace CustomProfile
{
    #region namespaces used

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;
    using System.Web.Profile;

    #endregion

    #region class

    public class SqlTableProfileProvider : ProfileProvider
    {
        #region global variables

        private string appName;
        private Guid appId;
        private bool appIdSet;
        private string sqlConnectionString;
        private int commandTimeout;
        private string dbTable;

        #endregion

        #region get configuration details

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (String.IsNullOrEmpty(name))
            {
                name = "SqlTableProfileProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "SqlTableProfileProvider");
            }
            base.Initialize(name, config);

            string temp = config["connectionStringName"];
            if (String.IsNullOrEmpty(temp))
            {
                throw new ProviderException("connectionStringName not specified");
            }
            sqlConnectionString = SqlStoredProcedureProfileProvider.GetConnectionString(temp);
            if (String.IsNullOrEmpty(sqlConnectionString))
            {
                throw new ProviderException("connectionStringName not specified");
            }

            appName = config["applicationName"];
            if (string.IsNullOrEmpty(appName))
            {
                appName = SqlStoredProcedureProfileProvider.GetDefaultAppName();
            }

            if (appName.Length > 256)
            {
                throw new ProviderException("Application name too long");
            }

            dbTable = config["table"];
            if (string.IsNullOrEmpty(dbTable))
            {
                throw new ProviderException("No table specified");
            }
            EnsureValidTableOrColumnName(dbTable);

            string timeout = config["commandTimeout"];
            if (string.IsNullOrEmpty(timeout) || !Int32.TryParse(timeout, out commandTimeout))
            {
                commandTimeout = 30;
            }

            config.Remove("commandTimeout");
            config.Remove("connectionStringName");
            config.Remove("applicationName");
            config.Remove("table");
            if (config.Count > 0)
            {
                string attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                {
                    throw new ProviderException("Unrecognized config attribute:" + attribUnrecognized);
                }
            }
        }

        #endregion

        #region set application name

        public override string ApplicationName
        {
            get { return appName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("ApplicationName");
                if (value.Length > 256)
                {
                    throw new ProviderException("Application name too long");
                }
                appName = value;
                appIdSet = false;
            }
        }

        #endregion

        #region set application id

        private Guid AppId
        {
            get
            {
                if (!appIdSet)
                {
                    SqlConnection conn = null;
                    try
                    {
                        conn = new SqlConnection(sqlConnectionString);
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("dbo.aspnet_Applications_CreateApplication", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@applicationname", ApplicationName);
                        cmd.Parameters.Add(CreateOutputParam("@ApplicationId", SqlDbType.UniqueIdentifier, 0));

                        cmd.ExecuteNonQuery();
                        appId = (Guid)cmd.Parameters["@ApplicationId"].Value;
                        appIdSet = true;
                    }
                    finally
                    {
                        if (conn != null)
                        {
                            conn.Close();
                        }
                    }

                }
                return appId;
            }
        }

        #endregion

        #region set timeout

        private int CommandTimeout
        {
            get { return commandTimeout; }
        }

        #endregion

        #region ensure valid db table and column name

        private static string s_legalChars = "_@#$";
        private static void EnsureValidTableOrColumnName(string name)
        {
            for (int i = 0; i < name.Length; ++i)
            {
                if (!Char.IsLetterOrDigit(name[i]) && s_legalChars.IndexOf(name[i]) == -1)
                {
                    throw new ProviderException("Table and column names cannot contain: " + name[i]);
                }
            }
        }

        #endregion

        #region get profile data from table

        private void GetProfileDataFromTable(SettingsPropertyCollection properties, SettingsPropertyValueCollection svc, string username, SqlConnection conn)
        {
            List<ProfileColumnData> columnData = new List<ProfileColumnData>(properties.Count);
            StringBuilder commandText = new StringBuilder("SELECT u.UserID");
            SqlCommand cmd = new SqlCommand(String.Empty, conn);

            int columnCount = 0;
            foreach (SettingsProperty prop in properties)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(prop);
                svc.Add(value);

                string persistenceData = prop.Attributes["CustomProviderData"] as string;
                // If table or column data is not found, ignore and continue
                if (String.IsNullOrEmpty(persistenceData))
                {
                    // REVIEW: Perhaps we should throw instead?
                    continue;
                }
                string[] chunk = persistenceData.Split(new char[] { ';' });
                if (chunk.Length != 2)
                {
                    // REVIEW: Perhaps we should throw instead?
                    continue;
                }
                string columnName = chunk[0];
                // REVIEW: Should we ignore case?
                SqlDbType datatype = (SqlDbType)Enum.Parse(typeof(SqlDbType), chunk[1], true);

                columnData.Add(new ProfileColumnData(columnName, value, null /* not needed for get */, datatype));
                commandText.Append(", ");
                commandText.Append("t." + columnName);
                ++columnCount;
            }

            commandText.Append(" FROM " + dbTable + " t, vw_aspnet_Users u WHERE u.ApplicationId = '").Append(AppId);
            commandText.Append("' AND u.UserName = LOWER(@Username) AND t.UserID = u.UserID");
            cmd.CommandText = commandText.ToString();
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Username", username);
            SqlDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
                //If no row exists in the database, then the default Profile values from configuration are used.
                if (reader.Read())
                {
                    Guid userId = reader.GetGuid(0);
                    for (int i = 0; i < columnData.Count; ++i)
                    {
                        object val = reader.GetValue(i + 1);
                        ProfileColumnData colData = columnData[i];
                        SettingsPropertyValue propValue = colData.PropertyValue;

                        // Only initialize a SettingsPropertyValue for non-null values
                        if (!(val is DBNull || val == null))
                        {
                            propValue.PropertyValue = val;
                            propValue.IsDirty = false;
                            propValue.Deserialized = true;
                        }
                    }

                    // need to close reader before we try to update the user
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                    UpdateLastActivityDate(conn, userId);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        #endregion

        #region update last activity date

        private static void UpdateLastActivityDate(SqlConnection conn, Guid userId)
        {
            SqlCommand cmd = new SqlCommand("UPDATE aspnet_Users SET LastActivityDate = @LastUpdatedDate WHERE UserId = '" + userId + "'", conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@LastUpdatedDate", DateTime.UtcNow);
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Dispose();
            }
        }

        #endregion

        #region get property values

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection svc = new SettingsPropertyValueCollection();

            if (collection == null || collection.Count < 1 || context == null)
            {
                return svc;
            }

            string username = (string)context["UserName"];
            if (String.IsNullOrEmpty(username))
            {
                return svc;
            }

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(sqlConnectionString);
                conn.Open();

                GetProfileDataFromTable(collection, svc, username, conn);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return svc;
        }

        #endregion

        #region profile column data

        // Container struct for use in aggregating columns for queries
        private struct ProfileColumnData
        {
            public string ColumnName;
            public SettingsPropertyValue PropertyValue;
            public object Value;
            public SqlDbType DataType;

            public ProfileColumnData(string col, SettingsPropertyValue pv, object val, SqlDbType type)
            {
                EnsureValidTableOrColumnName(col);
                ColumnName = col;
                PropertyValue = pv;
                Value = val;
                DataType = type;
            }
        }

        #endregion

        #region set property values

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            string username = (string)context["UserName"];
            bool userIsAuthenticated = (bool)context["IsAuthenticated"];

            if (username == null || username.Length < 1 || collection.Count < 1)
            {
                return;
            }

            SqlConnection conn = null;
            SqlDataReader reader = null;
            SqlCommand cmd = null;
            try
            {
                bool anyItemsToSave = false;

                // First make sure we have at least one item to save
                foreach (SettingsPropertyValue pp in collection)
                {
                    if (pp.IsDirty)
                    {
                        if (!userIsAuthenticated)
                        {
                            bool allowAnonymous = (bool)pp.Property.Attributes["AllowAnonymous"];
                            if (!allowAnonymous)
                            {
                                continue;
                            }
                        }
                        anyItemsToSave = true;
                        break;
                    }
                }

                if (!anyItemsToSave)
                {
                    return;
                }

                conn = new SqlConnection(sqlConnectionString);
                conn.Open();

                List<ProfileColumnData> columnData = new List<ProfileColumnData>(collection.Count);

                foreach (SettingsPropertyValue pp in collection)
                {
                    if (!userIsAuthenticated)
                    {
                        bool allowAnonymous = (bool)pp.Property.Attributes["AllowAnonymous"];
                        if (!allowAnonymous)
                        {
                            continue;
                        }
                    }

                    // Normal logic for original SQL provider
                    // if (!pp.IsDirty && pp.UsingDefaultValue) // Not fetched from DB and not written to

                    // Can eliminate unnecessary updates since we are using a table
                    if (!pp.IsDirty)
                    {
                        continue;
                    }

                    string persistenceData = pp.Property.Attributes["CustomProviderData"] as string;
                    // If we can't find the table/column info we will ignore this data
                    if (String.IsNullOrEmpty(persistenceData))
                    {
                        // REVIEW: Perhaps we should throw instead?
                        continue;
                    }
                    string[] chunk = persistenceData.Split(new char[] { ';' });
                    if (chunk.Length != 2)
                    {
                        // REVIEW: Perhaps we should throw instead?
                        continue;
                    }
                    string columnName = chunk[0];
                    // REVIEW: Should we ignore case?
                    SqlDbType datatype = (SqlDbType)Enum.Parse(typeof(SqlDbType), chunk[1], true);

                    object value = null;

                    // REVIEW: Is this handling null case correctly?
                    if (pp.Deserialized && pp.PropertyValue == null)
                    { // is value null?
                        value = DBNull.Value;
                    }
                    else
                    {
                        value = pp.PropertyValue;
                    }

                    // REVIEW: Might be able to ditch datatype
                    columnData.Add(new ProfileColumnData(columnName, pp, value, datatype));
                }

                // Figure out userid, if we don't find a userid, go ahead and create a user in the aspnetUsers table
                Guid userId = Guid.Empty;
                cmd = new SqlCommand("SELECT u.UserId FROM vw_aspnet_Users u WHERE u.ApplicationId = '" + AppId + "' AND u.UserName = LOWER(@Username)", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Username", username);
                try
                {
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        userId = reader.GetGuid(0);
                    }
                    else
                    {
                        reader.Close();
                        cmd.Dispose();
                        reader = null;

                        cmd = new SqlCommand("dbo.aspnet_Users_CreateUser", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ApplicationId", AppId);
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@IsUserAnonymous", !userIsAuthenticated);
                        cmd.Parameters.AddWithValue("@LastActivityDate", DateTime.UtcNow);
                        cmd.Parameters.Add(CreateOutputParam("@UserId", SqlDbType.UniqueIdentifier, 16));

                        cmd.ExecuteNonQuery();
                        userId = (Guid)cmd.Parameters["@userid"].Value;
                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                    cmd.Dispose();
                }

                // Figure out if the row already exists in the table and use appropriate SELECT/UPDATE
                cmd = new SqlCommand(String.Empty, conn);
                StringBuilder sqlCommand = new StringBuilder("IF EXISTS (SELECT 1 FROM ").Append(dbTable);
                sqlCommand.Append(" WHERE UserId = @UserId) ");
                cmd.Parameters.AddWithValue("@UserId", userId);

                // Build up strings used in the query
                StringBuilder columnStr = new StringBuilder();
                StringBuilder valueStr = new StringBuilder();
                StringBuilder setStr = new StringBuilder();
                int count = 0;
                foreach (ProfileColumnData data in columnData)
                {
                    columnStr.Append(", ");
                    valueStr.Append(", ");
                    columnStr.Append(data.ColumnName);
                    string valueParam = "@Value" + count;
                    valueStr.Append(valueParam);
                    cmd.Parameters.AddWithValue(valueParam, data.Value);

                    // REVIEW: Can't update Timestamps?
                    if (data.DataType != SqlDbType.Timestamp)
                    {
                        if (count > 0)
                        {
                            setStr.Append(",");
                        }
                        setStr.Append(data.ColumnName);
                        setStr.Append("=");
                        setStr.Append(valueParam);
                    }

                    ++count;
                }
                columnStr.Append(",LastUpdatedDate ");
                valueStr.Append(",@LastUpdatedDate");
                setStr.Append(",LastUpdatedDate=@LastUpdatedDate");
                cmd.Parameters.AddWithValue("@LastUpdatedDate", DateTime.UtcNow);

                sqlCommand.Append("BEGIN UPDATE ").Append(dbTable).Append(" SET ").Append(setStr.ToString());
                sqlCommand.Append(" WHERE UserId = '").Append(userId).Append("'");

                sqlCommand.Append("END ELSE BEGIN INSERT ").Append(dbTable).Append(" (UserId").Append(columnStr.ToString());
                sqlCommand.Append(") VALUES ('").Append(userId).Append("'").Append(valueStr.ToString()).Append(") END");

                cmd.CommandText = sqlCommand.ToString();
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                // Need to close reader before we try to update
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                UpdateLastActivityDate(conn, userId);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        #endregion

        #region create input/output parameters

        private static SqlParameter CreateInputParam(string paramName, SqlDbType dbType, object objValue)
        {
            SqlParameter param = new SqlParameter(paramName, dbType);
            if (objValue == null)
            {
                objValue = String.Empty;
            }
            param.Value = objValue;
            return param;
        }

        private static SqlParameter CreateOutputParam(string paramName, SqlDbType dbType, int size)
        {
            SqlParameter param = new SqlParameter(paramName, dbType);
            param.Direction = ParameterDirection.Output;
            param.Size = size;
            return param;
        }

        #endregion

        #region delete profiles

        // Management APIs from ProfileProvider class
        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException("profiles");
            }

            if (profiles.Count < 1)
            {
                throw new ArgumentException("Profiles collection is empty");
            }

            string[] usernames = new string[profiles.Count];

            int iter = 0;
            foreach (ProfileInfo profile in profiles)
            {
                usernames[iter++] = profile.UserName;
            }

            return DeleteProfiles(usernames);
        }

        public override int DeleteProfiles(string[] usernames)
        {
            if (usernames == null || usernames.Length < 1)
            {
                return 0;
            }

            int numProfilesDeleted = 0;
            bool beginTranCalled = false;
            try
            {
                SqlConnection conn = null;
                try
                {
                    conn = new SqlConnection(sqlConnectionString);
                    conn.Open();

                    SqlCommand cmd;
                    int numUsersRemaing = usernames.Length;
                    while (numUsersRemaing > 0)
                    {
                        cmd = new SqlCommand(String.Empty, conn);
                        cmd.Parameters.AddWithValue("@UserName0", usernames[usernames.Length - numUsersRemaing]);
                        StringBuilder allUsers = new StringBuilder("@UserName0");
                        numUsersRemaing--;

                        int userIndex = 1;
                        for (int iter = usernames.Length - numUsersRemaing; iter < usernames.Length; iter++)
                        {
                            // REVIEW: Should we check length of command string instead of parameter lengths?
                            if (allUsers.Length + usernames[iter].Length + 3 >= 4000)
                                break;
                            string userNameParam = "@UserName" + userIndex;
                            allUsers.Append(",");
                            allUsers.Append(userNameParam);
                            cmd.Parameters.AddWithValue(userNameParam, usernames[iter]);
                            numUsersRemaing--;
                            ++userIndex;
                        }

                        // We don't need to start a transaction if we can finish this in one sql command
                        if (!beginTranCalled && numUsersRemaing > 0)
                        {
                            SqlCommand beginCmd = new SqlCommand("BEGIN TRANSACTION", conn);
                            beginCmd.ExecuteNonQuery();
                            beginTranCalled = true;
                        }


                        cmd.CommandText = "DELETE FROM " + dbTable + " WHERE UserId IN ( SELECT u.UserId FROM vw_aspnet_Users u WHERE u.ApplicationId = '" + AppId + "' AND u.UserName IN (" + allUsers.ToString() + "))";
                        cmd.CommandTimeout = CommandTimeout;
                        numProfilesDeleted += cmd.ExecuteNonQuery();
                    }

                    if (beginTranCalled)
                    {
                        cmd = new SqlCommand("COMMIT TRANSACTION", conn);
                        cmd.ExecuteNonQuery();
                        beginTranCalled = false;
                    }
                }
                catch
                {
                    if (beginTranCalled)
                    {
                        SqlCommand cmd = new SqlCommand("ROLLBACK TRANSACTION", conn);
                        cmd.ExecuteNonQuery();
                        beginTranCalled = false;
                    }
                    throw;
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
            }
            catch
            {
                throw;
            }
            return numProfilesDeleted;
        }

        #endregion

        #region generate select/delete query

        private string GenerateQuery(bool delete, ProfileAuthenticationOption authenticationOption)
        {
            StringBuilder cmdStr = new StringBuilder(200);
            if (delete)
            {
                cmdStr.Append("DELETE FROM ");
            }
            else
            {
                cmdStr.Append("SELECT COUNT(*) FROM ");
                cmdStr.Append(dbTable);
                cmdStr.Append(" WHERE UserId IN (SELECT u.UserId FROM vw_aspnet_Users u WHERE u.ApplicationId = '").Append(AppId);
                cmdStr.Append("' AND (u.LastActivityDate <= @InactiveSinceDate)");
            }

            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    cmdStr.Append(" AND u.IsAnonymous = 1");
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    cmdStr.Append(" AND u.IsAnonymous = 0");
                    break;
                case ProfileAuthenticationOption.All:
                    // Want to delete all profiles here, so nothing more needed
                    break;
            }
            cmdStr.Append(")");
            return cmdStr.ToString();
        }

        #endregion

        #region delete inactive profiles

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            try
            {
                SqlConnection conn = null;
                SqlCommand cmd = null;
                try
                {
                    conn = new SqlConnection(sqlConnectionString);
                    conn.Open();

                    cmd = new SqlCommand(GenerateQuery(true, authenticationOption), conn);
                    cmd.CommandTimeout = CommandTimeout;
                    cmd.Parameters.Add(CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime()));

                    return cmd.ExecuteNonQuery();
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region get number of inactive profiles

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                conn = new SqlConnection(sqlConnectionString);
                conn.Open();

                cmd = new SqlCommand(GenerateQuery(false, authenticationOption), conn);
                cmd.CommandTimeout = CommandTimeout;
                cmd.Parameters.Add(CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime()));

                object o = cmd.ExecuteScalar();
                if (o == null || !(o is int))
                {
                    return 0;
                }
                return (int)o;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
        }

        #endregion

        #region generate temp insert query for GetProfiles()

        // TODO: Implement size
        private StringBuilder GenerateTempInsertQueryForGetProfiles(ProfileAuthenticationOption authenticationOption)
        {
            StringBuilder cmdStr = new StringBuilder(200);
            cmdStr.Append("INSERT INTO #PageIndexForProfileUsers (UserId) ");
            cmdStr.Append("SELECT u.UserId FROM vw_aspnet_Users u, ").Append(dbTable);
            cmdStr.Append(" p WHERE ApplicationId = '").Append(AppId);
            cmdStr.Append("' AND u.UserId = p.UserId");
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    cmdStr.Append(" AND u.IsAnonymous = 1");
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    cmdStr.Append(" AND u.IsAnonymous = 0");
                    break;
                case ProfileAuthenticationOption.All:
                    // Want to delete all profiles here, so nothing more needed
                    break;
            }
            return cmdStr;
        }

        #endregion

        #region Get All Profiles

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            return GetProfilesForQuery(null, pageIndex, pageSize, insertQuery, out totalRecords);
        }

        #endregion

        #region get all inactive profiles

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            insertQuery.Append(" AND u.LastActivityDate <= @InactiveSinceDate");
            SqlParameter[] args = new SqlParameter[1];
            args[0] = CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime());
            return GetProfilesForQuery(args, pageIndex, pageSize, insertQuery, out totalRecords);
        }

        #endregion

        #region find profiles by user name

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            insertQuery.Append(" AND u.UserName LIKE LOWER(@UserName)");
            SqlParameter[] args = new SqlParameter[1];
            args[0] = CreateInputParam("@UserName", SqlDbType.NVarChar, usernameToMatch);
            return GetProfilesForQuery(args, pageIndex, pageSize, insertQuery, out totalRecords);
        }

        #endregion

        #region find inactive profiles by user name

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            insertQuery.Append(" AND u.UserName LIKE LOWER(@UserName) AND u.LastActivityDate <= @InactiveSinceDate");
            SqlParameter[] args = new SqlParameter[2];
            args[0] = CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime());
            args[1] = CreateInputParam("@UserName", SqlDbType.NVarChar, usernameToMatch);
            return GetProfilesForQuery(args, pageIndex, pageSize, insertQuery, out totalRecords);
        }

        #endregion

        #region private methods - get profiles for query

        private ProfileInfoCollection GetProfilesForQuery(SqlParameter[] insertArgs, int pageIndex, int pageSize, StringBuilder insertQuery, out int totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException("pageIndex");
            }
            if (pageSize < 1)
            {
                throw new ArgumentException("pageSize");
            }

            long lowerBound = (long)pageIndex * pageSize;
            long upperBound = lowerBound + pageSize - 1;
            if (upperBound > Int32.MaxValue)
            {
                throw new ArgumentException("pageIndex and pageSize");
            }

            SqlConnection conn = null;
            SqlDataReader reader = null;
            SqlCommand cmd = null;
            try
            {
                conn = new SqlConnection(sqlConnectionString);
                conn.Open();

                StringBuilder cmdStr = new StringBuilder(200);
                // Create a temp table TO store the select results
                cmd = new SqlCommand("CREATE TABLE #PageIndexForProfileUsers(IndexId int IDENTITY (0, 1) NOT NULL, UserId uniqueidentifier)", conn);
                cmd.CommandTimeout = CommandTimeout;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                insertQuery.Append(" ORDER BY UserName");
                cmd = new SqlCommand(insertQuery.ToString(), conn);
                cmd.CommandTimeout = CommandTimeout;
                if (insertArgs != null)
                {
                    foreach (SqlParameter arg in insertArgs)
                    {
                        cmd.Parameters.Add(arg);
                    }
                }

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmdStr = new StringBuilder(200);
                cmdStr.Append("SELECT u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate FROM vw_aspnet_Users u, ").Append(dbTable);
                cmdStr.Append(" p, #PageIndexForProfileUsers i WHERE u.UserId = p.UserId AND p.UserId = i.UserId AND i.IndexId >= ");
                cmdStr.Append(lowerBound).Append(" AND i.IndexId <= ").Append(upperBound);
                cmd = new SqlCommand(cmdStr.ToString(), conn);
                cmd.CommandTimeout = CommandTimeout;

                reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                ProfileInfoCollection profiles = new ProfileInfoCollection();
                while (reader.Read())
                {
                    string username;
                    DateTime dtLastActivity, dtLastUpdated = DateTime.UtcNow;
                    bool isAnon;

                    username = reader.GetString(0);
                    isAnon = reader.GetBoolean(1);
                    dtLastActivity = DateTime.SpecifyKind(reader.GetDateTime(2), DateTimeKind.Utc);
                    dtLastUpdated = DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc);
                    profiles.Add(new ProfileInfo(username, isAnon, dtLastActivity, dtLastUpdated, 0));
                }
                totalRecords = profiles.Count;

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                cmd.Dispose();

                // Cleanup, REVIEW: should move to finally?
                cmd = new SqlCommand("DROP TABLE #PageIndexForProfileUsers", conn);
                cmd.ExecuteNonQuery();

                return profiles;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                }

                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
        }

        #endregion
    }

    #endregion
}

#endregion