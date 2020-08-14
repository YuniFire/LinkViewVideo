using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Configuration;
using System.Net.Mime; 

namespace MMSMessaging
{
    public class Messaging
    {
        //Users are not required to enter the MMS email address on their recipient list
        //The method below can either send the email directly to the email address if specified
        //or it can use a dirty email pattern to attempt to locate the correct email address
        //If there is no email address specified for the recipient (only the phone number)
        //This is a dirty email send program, it is used to find the correct email address and on success, break 
        //Add a parameter to send the MMS attachment media file
        private string attachment;

        public string Attachment
        {
            get { return attachment; }
            set { attachment = value; }
        }
        private string formalName;

        public string FormalName
        {
            get { return formalName; }
            set { formalName = value; }
        }
        
        }
    }

