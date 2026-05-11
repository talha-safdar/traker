using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traker.ViewModels
{
    class MessageBoxViewModel : Screen
    {
        #region Private View Variables
        private int _symbol; // 0=info, 1=warning, 2=error
        private string _headMessage; 
        private string _message;
        private int _buttonStyle; // 0=OK, 1=CancelOK
        #endregion

        public MessageBoxViewModel(int symbol, string headMessage, string message, int buttonStyle)
        {
            _symbol = symbol;
            _headMessage = headMessage;
            _message = message;
            _buttonStyle = buttonStyle;
        }

        #region Public Functions
        public async Task OK()
        {
            await TryCloseAsync();
        }
        #endregion

        #region Public View Variables
        public int Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;
                NotifyOfPropertyChange(() => Symbol);
            }
        }

        public string HeadMessage
        {
            get { return _headMessage; }
            set
            {
                _headMessage = value;
                NotifyOfPropertyChange(() => HeadMessage);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public int ButtonStyle
        {
            get { return _buttonStyle; }
            set
            {
                _buttonStyle = value;
                NotifyOfPropertyChange(() => ButtonStyle);
            }
        }
        #endregion
    }
}
