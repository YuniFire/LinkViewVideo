//-------------------------------------------------------------------------------
//  toggle checkboxes javascript function
//-------------------------------------------------------------------------------
function CheckAll(CheckBoxId, CheckVal) {
    for (i = 0; i < document.forms[0].elements.length; i++) //Loop through all form elements
    {
        elm = document.forms[0].elements[i];
        if (elm.type == 'checkbox') //Check if the element is a checkbox
        {
            var str = elm.name;
            if (str.indexOf(CheckBoxId) != -1) //See if checkbox has ID which we're looking for
            {
                elm.click();
                elm.checked = CheckVal; //Set the checked value
            }
        }
    }
}
