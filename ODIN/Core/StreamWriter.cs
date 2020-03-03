using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IslaBot
{
    public class TextBoxStreamWriter : StringWriter
    {
        #region Properties
        private ListBox _output;
        protected StreamWriter writer;
        protected MemoryStream mem;
        #endregion

        #region Constructor
        public TextBoxStreamWriter(ListBox box)
        {
            _output = box;
            mem = new MemoryStream(1000000);
            writer = new StreamWriter(mem)
            {
                AutoFlush = true
            };
        }
        #endregion

        #region Override Mathods
        // override methods
        public override void Write(char value)
        {
            
            _output.Dispatcher.BeginInvoke((Action)(() => { _output.Items.Insert(0, value); }));
            
        }

        // override methods
        public override void Write(string value)
        {
            
            _output.Dispatcher.BeginInvoke((Action)(() => { _output.Items.Insert(0, value); }));
        }

        public override void WriteLine(string value)
        {
           
            _output.Dispatcher.BeginInvoke((Action)(() => { _output.Items.Insert(0, value); })); 
           
        }
        #endregion

    }
}
