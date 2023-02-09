using QFramework;

namespace AbyssDemo
{
    public class SwitchCellCheckCommand : AbstractCommand
    {
        private bool _checked = false;
        public SwitchCellCheckCommand(bool @checked)
        {
            _checked = @checked;
        }

        protected override void OnExecute()
        {
            this.SendEvent<OnCellCheck>(new OnCellCheck() { enable= _checked });
        }
    }
}