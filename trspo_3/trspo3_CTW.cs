// + 
namespace trspo_3
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    [Guid("6c97d4e8-ea97-453a-a9ef-c54b03024a16")]
    public class trspo3_CTW : ToolWindowPane
    {
        public trspo3_CTW() : base(null)
        {
            this.Caption = "trspo3_CTW";
            this.Content = new trspo3_CTWControl();
        }
    }
}
