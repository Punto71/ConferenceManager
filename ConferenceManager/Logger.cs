using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceManager {
     public class Logger {

         public static event EventHandler MessageListChanged;

         private static List<string> _messageList;

         public static List<string> MessageList {
             get {
                 if (_messageList == null)
                     _messageList = new List<string>();
                 return _messageList;
             }
         }


         public static void addError(string text) {
             addMessage(createMessage(text, "ERROR"));
         }

         public static void addInfo(string text) {
             addMessage(createMessage(text, "INFO"));
         }

         public static void addWarning(string text) {
             addMessage(createMessage(text, "WARNING"));
         }

         private static void addMessage(string message) {
             MessageList.Add(message);
             if (MessageListChanged != null)
                 MessageListChanged.Invoke(MessageList, new EventArgs());
         }

         private static string createMessage(string text, string type) {
             return string.Format("{0} [{1}] - {2}", DateTime.Now, type, text);
         }
    }
}
