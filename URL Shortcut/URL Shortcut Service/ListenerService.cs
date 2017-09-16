using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace URL_Shortcut_Service
{
    public partial class ListenerService : ServiceBase
    {
        public ListenerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        // An entry point for debugging purposes
        public void GoDebug(string[] args)
        {
            this.OnStart(args);
        }
    }
}
