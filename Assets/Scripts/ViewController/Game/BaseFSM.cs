using UnityEngine;
using QFramework;
using DG.Tweening;

namespace AbyssDemo
{
    public enum TurnState
    {
        READY,
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
    public class BaseFSM : MonoBehaviour,IController
    {
        public BaseUnitSO baseUnit;
        
        private GameObject goTarget;
        public GameObject GoTarget { get { return goTarget; } set { goTarget = value; } }
        //public float curActionTime;

        private Tweener tweener1;
        private Tweener tweener2;
        private Tweener tweener3;
        private Tweener tweener4;
        private Tweener tweener5;

        private IRuntimeUnitModel runtimeUnitModel;

        private RuntimeUnitInfo runtimeUnitInfo;

        private SpriteRenderer spriteRenderer;

        public FSM<TurnState> SimpleUnitFSM = new FSM<TurnState>();
        
        void Start()
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            runtimeUnitInfo = new RuntimeUnitInfo(baseUnit,SimpleUnitFSM,this.gameObject.GetHashCode());
            runtimeUnitInfo.OnHited.Register(HitHP).UnRegisterWhenGameObjectDestroyed(gameObject);
            runtimeUnitInfo.OnHealed.Register(HealHP).UnRegisterWhenGameObjectDestroyed(gameObject);
            runtimeUnitInfo.OnDestroy.Register(() => Destroy(this.gameObject)).UnRegisterWhenGameObjectDestroyed(gameObject);
            runtimeUnitInfo.OnDodged.Register(Dodged).UnRegisterWhenGameObjectDestroyed(gameObject);

            runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
            runtimeUnitModel.AddRuntimeUnit(this.gameObject.GetHashCode(), runtimeUnitInfo);

            TypeEventSystem.Global.Send(new OnUnitInitEvent() { gameObject = this.gameObject });

            SimpleUnitFSM.AddState(TurnState.READY, new ReadyState(SimpleUnitFSM, this));
            SimpleUnitFSM.AddState(TurnState.PROCESSING, new ProcessingState(SimpleUnitFSM, this));
            //SimpleUnitFSM.AddState(TurnState.CHOOSEACTION, new ChooseActionState(SimpleUnitFSM, this));
            SimpleUnitFSM.AddState(TurnState.WAITING, new WaitingState(SimpleUnitFSM, this));
            SimpleUnitFSM.AddState(TurnState.ACTION, new ActionState(SimpleUnitFSM, this));
            SimpleUnitFSM.AddState(TurnState.DEAD,new DeadState(SimpleUnitFSM, this));
            SimpleUnitFSM.StartState(TurnState.READY);
            //curHP = baseUnit.HP;
           
        }
        public void HitHP(float damage)
        {
            
            var y = transform.position.y;
            tweener1 = transform.DOMoveY(y+0.3f,0.05f).SetEase(Ease.Linear).SetLoops(2,LoopType.Yoyo);
            tweener2 = spriteRenderer.DOColor(Color.red,0.1f).SetLoops(2,LoopType.Yoyo);
            
        }
        public void HealHP(float heal)
        {
            var y = transform.position.y;
            tweener3 = transform.DOMoveY(y + 0.3f, 0.05f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            tweener4 = spriteRenderer.DOColor(Color.green, 0.1f).SetLoops(2, LoopType.Yoyo);
        }
        public void Dodged()
        {
            tweener5 = spriteRenderer.DOColor(new Color(0.34f,0.34f,0.34f), 0.1f).SetLoops(2, LoopType.Yoyo);
        }

        // Update is called once per frame
        void Update()
        {
            SimpleUnitFSM.Update();
        }
        private void FixedUpdate()
        {
            SimpleUnitFSM.FixedUpdate();
        }
        private void OnGUI()
        {
            SimpleUnitFSM.OnGUI();
            
        }
        private void OnDestroy()
        {
            tweener1.Kill();
            tweener2.Kill();
            tweener3.Kill();
            tweener4.Kill();
            tweener5.Kill();
            SimpleUnitFSM.Clear();
            runtimeUnitModel.RemoveRuntimeUnit(this.gameObject.GetHashCode());
        }
        private void OnMouseOver()
        {
            if(Input.GetMouseButtonDown(1)&& BattleSystem.Instance.battleStates == BattleSystem.PerformAction.READY)
            {
                var mUnit = runtimeUnitModel.GetRuntimeUnit(this.gameObject.GetHashCode());
                mUnit.OnDestroy.Trigger();
            }
        }
        private void OnMouseEnter()
        {
            TypeEventSystem.Global.Send(new OnUnitHover() { unitHovering = runtimeUnitInfo, IsCanvasOn = true });
        }
        private void OnMouseExit()
        {
            TypeEventSystem.Global.Send(new OnUnitHover() { unitHovering = null, IsCanvasOn = false });

        }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }
        
    }
}