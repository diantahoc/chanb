using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.DataTypes
{
    public class OPData
    {

        private string _name = Language.Lang.anonymous;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _name = Language.Lang.anonymous;
                    this.Trip = "";
                }
                else
                {
                    string trimmed = value.Trim();

                    if (trimmed.Length > Common.Limits.MaximumFieldLength)
                    {
                        trimmed = trimmed.Remove(Common.Limits.MaximumFieldLength - 1);
                    }

                    this.Trip = find_trip(trimmed);
                    _name = trimmed;
                }
            }
        }

        private string find_trip(string name)
        {
            if (name.Contains('#'))
            {
                return Tripcode.TripComputer.ComputeTrip(name.Split('#')[1]);
            }
            else { return ""; }
        }

        public string Trip { get; private set; }

        private string _email = "";

        public string Email
        {
            get { return _email; }
            set
            {
                string trimmed = value.Trim();
                if (trimmed.Length > Common.Limits.MaximumFieldLength)
                {
                    trimmed = trimmed.Remove(Common.Limits.MaximumFieldLength - 1);
                }
                _email = trimmed;
            }
        }

        private string _subject = "";

        public string Subject
        {
            get { return _subject; }
            set
            {
                string trimmed = value.Trim();
                if (trimmed.Length > Common.Limits.MaximumFieldLength)
                {
                    trimmed = trimmed.Remove(Common.Limits.MaximumFieldLength - 1);
                }
                _subject = trimmed;
            }
        }

        private string _comment;
        public string Comment
        {
            get { return this._comment; }

            set { this._comment = HttpUtility.HtmlEncode(value); }
        }

        private string _password = "";

        public string Password
        {
            get { return _password; }
            set
            {
                string trimmed = value.Trim();
                if (trimmed.Length > Common.Limits.MaximumFieldLength)
                {
                    trimmed = trimmed.Remove(Common.Limits.MaximumFieldLength - 1);
                }
                _password = trimmed;
            }
        }

        public bool HasFile { get; set; }

        public string IP { get; set; }

        public string UserAgent { get; set; }

        public DateTime Time { get; set; }

        public OPData() 
        {
            //setup values with no defaults
            this.Time = DateTime.Now;
            this.UserAgent = "";
            this.IP = "";
            this.HasFile = false;
        }

    }
}