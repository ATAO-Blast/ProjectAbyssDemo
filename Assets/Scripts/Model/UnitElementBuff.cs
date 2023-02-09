namespace AbyssDemo
{
    public interface IUnitElementBuff
    {
        float BuffInitTime { get; set; }
        int BurningCount { get; set; }
        int PosionCount { get; set; }

        ElementBuffType ElementBuff { get; set; }

        RuntimeUnitInfo BuffProvider { get; set; }
        float BuffAtk { get; set; }
    }
    public struct UnitElementBuff : IUnitElementBuff
    {
        private float buffInitTime;
        private float buffAtk;
        private int burningCount;
        private int posionCount;
        private RuntimeUnitInfo buffProvider;
        private ElementBuffType elementBuffType;
        public UnitElementBuff(RuntimeUnitInfo buffProvider, float buffAtk,int burningCount=4,int posionCount=6,ElementBuffType elementBuff= ElementBuffType.None,float buffInitTime = 0f)
        {
            this.burningCount = burningCount;
            this.posionCount = posionCount;
            this.elementBuffType = elementBuff;
            this.buffAtk= buffAtk;
            this.buffProvider= buffProvider;
            this.buffInitTime= buffInitTime;
        }
        public float BuffInitTime { get { return buffInitTime; } set { buffInitTime = value; } }
        public int BurningCount { get { return burningCount; } set { burningCount = value; } }
        public int PosionCount { get { return posionCount; } set { posionCount = value; } }
        public RuntimeUnitInfo BuffProvider { get { return buffProvider; } set { buffProvider = value; } }
        public ElementBuffType ElementBuff { get { return elementBuffType; } set { elementBuffType = value; } }
        public float BuffAtk { get { return buffAtk; } set { buffAtk = value; } }

    }
}