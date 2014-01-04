using System.Windows.Forms;

namespace LolBackup
{
    // override base class to allow double buffering. this removes flickering when updating control content.
    public class ListViewFast : ListView
    {
        public ListViewFast() 
        {
            this.DoubleBuffered = true;
        }

        protected override sealed bool DoubleBuffered
        {
            get { return base.DoubleBuffered; }
            set { base.DoubleBuffered = value; }
        }
    }
}
