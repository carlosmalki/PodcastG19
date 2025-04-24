using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastAppG19
{
    public class Validation
    {
        public static bool NamnKontroll(string name, Label messageLabel)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                messageLabel.ForeColor = Color.Red;
                messageLabel.Text = ("Namnet får inte lämnas tomt!");
                return false;
            }
            else if (name.Length > 15)
            {
                messageLabel.ForeColor = Color.Red;
                messageLabel.Text = ("Namnet överskrider gränsen på 15 tecken!");
                return false;
            }
            messageLabel.Text = "";
            return true;
        }
    }

}
