using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications.Core;

namespace ToastNotifier.CustomNotificationMessage
{
    public class CustomMessageViewModel : NotificationBase, INotifyPropertyChanged
    {
        private CustomMessage _displayPart;

        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new CustomMessage(this));

        public CustomMessageViewModel(string title, string message, int level, MessageOptions messageOptions) : base(message, messageOptions)
        {
            Title = title;
            Message = message;
            Background = GetBackgroundColor(level);
        }

        private string GetBackgroundColor(int level)
        {
            var color = string.Empty;
            switch (level) 
            {
                case 1:
                    color = "Red";
                    break;
                case 2:
                    color = "Orange";
                    break;
                case 3:
                    color = "Green";
                    break;
                case 4:
                    color = "DarkBlue";
                    break;
                case 5:
                    color = "Gray";
                    break;
                case 9:
                    color = "Purple";
                    break;
            }

            return color;
        }

        #region binding properties
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private string _background;

        public string Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
