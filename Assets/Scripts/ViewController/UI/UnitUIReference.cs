using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AbyssDemo
{
    public class UnitUIReference : MonoBehaviour,IController,IPointerDownHandler,IPointerUpHandler,IDragHandler,IPointerEnterHandler,IPointerExitHandler
    {
        public GameObject goToInit;
        private GameObject goInited;
        private IRuntimeUnitModel runtimeUnitModel;
        private bool isInHeroCell;
        private bool isInEnemyCell;
        private int bodySize = 1;
        private GridSystem gridSystem;
        public void OnDrag(PointerEventData eventData)
        {
            
            CellSnap();
        }
        private void CellSnap()
        {
            var mPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var nPosition = new Vector3(mPosition.x, mPosition.y, 0);
            if(isInEnemyCell) { goInited.GetComponent<SpriteRenderer>().flipX = true; }
            else if (isInHeroCell) { goInited.GetComponent<SpriteRenderer>().flipX = false; }
            if (bodySize == 2)
            {
                goInited.transform.position = gridSystem.HeroGridCell.TwoCellSnap2D(nPosition, out isInHeroCell);
                if (isInHeroCell) return;
                goInited.transform.position = gridSystem.EnemyGridCell.TwoCellSnap2D(nPosition, out isInEnemyCell);
            }
            else if (bodySize == 3)
            {
                goInited.transform.position = gridSystem.HeroGridCell.ThreeCellSnap2D(nPosition, out isInHeroCell);
                if (isInHeroCell) return;
                goInited.transform.position = gridSystem.EnemyGridCell.ThreeCellSnap2D(nPosition, out isInEnemyCell);
            }
            else if (bodySize == 4)
            {
                goInited.transform.position = gridSystem.HeroGridCell.FourCellSnap2D(nPosition, out isInHeroCell);
                if (isInHeroCell) return;
                goInited.transform.position = gridSystem.EnemyGridCell.FourCellSnap2D(nPosition, out isInEnemyCell);
            }
            else if (bodySize == 6)
            {
                goInited.transform.position = gridSystem.HeroGridCell.SixCellSnap2D(nPosition, out isInHeroCell);
                if (isInHeroCell) return;
                goInited.transform.position = gridSystem.EnemyGridCell.SixCellSnap2D(nPosition, out isInEnemyCell);
            }
            else
            {
                goInited.transform.position = gridSystem.HeroGridCell.OneCellSnapUpdate2D(nPosition, out isInHeroCell);
                if (isInHeroCell) return;
                goInited.transform.position = gridSystem.EnemyGridCell.OneCellSnapUpdate2D(nPosition, out isInEnemyCell);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                goInited = GameObject.Instantiate(goToInit);
                var goFSM = goInited.gameObject.GetComponent<BaseFSM>();
                bodySize = goFSM.baseUnit.bodySize;
                TypeEventSystem.Global.Send(new OnAtlasUIHover() { baseUnitSO = null,IsCanvasOn = false});
                this.SendCommand<SwitchCellCheckCommand>(new SwitchCellCheckCommand(true));
                gridSystem = this.GetSystem<GridSystem>();
                var mPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                goInited.transform.position = new Vector3(mPosition.x, mPosition.y, 0);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            var goFSM = goToInit.GetComponent<BaseFSM>();
            TypeEventSystem.Global.Send<OnAtlasUIHover>(new OnAtlasUIHover() { baseUnitSO = goFSM.baseUnit, IsCanvasOn = true });
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            TypeEventSystem.Global.Send<OnAtlasUIHover>(new OnAtlasUIHover() { baseUnitSO = null, IsCanvasOn = false });
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.SendCommand(new SwitchCellCheckCommand(false));
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, 20);
            var unitRuntimeInfo = runtimeUnitModel.GetRuntimeUnit(goInited.GetHashCode());

            if (isInHeroCell && !hit.collider)
            {
                InitHeroGoThread();
                InitHeroGridIndex();
                goInited.gameObject.tag = "Hero";
                unitRuntimeInfo.Group = "Hero";
                unitRuntimeInfo.StartPosition = goInited.transform.position;
                goInited.gameObject.layer = 0;
            }
            else if (isInEnemyCell && !hit.collider)
            {
                InitEnemyGoThread();
                InitEnemyGridIndex();
                goInited.gameObject.tag = "Enemy";
                unitRuntimeInfo.Group = "Enemy";
                unitRuntimeInfo.StartPosition = goInited.transform.position;
                goInited.gameObject.layer = 0;
            }
            else unitRuntimeInfo.OnDestroy.Trigger();
        }

        void InitEnemyGridIndex()
        {
            var x = gridSystem.EnemyGridCell.GetX(goInited.transform.position);
            var y = gridSystem.EnemyGridCell.GetY(goInited.transform.position);
            var unitRuntimeInfo = runtimeUnitModel.GetRuntimeUnit(goInited.GetHashCode());
            if(bodySize == 1)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridY(y);
            }
            else if (bodySize == 2)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridX(x+ 1);
                unitRuntimeInfo.AddGridY(y);
            }
            else if(bodySize == 3)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridX(x + 1);
                unitRuntimeInfo.AddGridX(x + 2);

                unitRuntimeInfo.AddGridY(y);
            }
            else if (bodySize == 4)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridX(x + 1);
                unitRuntimeInfo.AddGridY(y);
                unitRuntimeInfo.AddGridY(y + 1);
            }
            else if (bodySize == 6)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridX(x + 1);
                unitRuntimeInfo.AddGridY(y);
                unitRuntimeInfo.AddGridY(y + 1);
                unitRuntimeInfo.AddGridY(y + 2);
            }
        }
        void InitHeroGridIndex()
        {
            var x = gridSystem.HeroGridCell.GetX(goInited.transform.position);
            var y = gridSystem.HeroGridCell.GetY(goInited.transform.position);
            var unitRuntimeInfo = runtimeUnitModel.GetRuntimeUnit(goInited.GetHashCode());
            if (bodySize == 1)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridY(y);
            }
            else if (bodySize == 2)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridX(x + 1);
                unitRuntimeInfo.AddGridY(y);
            }
            else if (bodySize == 3)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridX(x + 1);
                unitRuntimeInfo.AddGridX(x + 2);

                unitRuntimeInfo.AddGridY(y);
            }
            else if (bodySize == 4)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridX(x + 1);
                unitRuntimeInfo.AddGridY(y);
                unitRuntimeInfo.AddGridY(y + 1);
            }
            else if (bodySize == 4)
            {
                unitRuntimeInfo.AddGridX(x);
                unitRuntimeInfo.AddGridX(x + 1);
                unitRuntimeInfo.AddGridY(y);
                unitRuntimeInfo.AddGridY(y + 1);
                unitRuntimeInfo.AddGridY(y + 2);
            }
        }
        void InitEnemyGoThread()
        {
            var x = gridSystem.EnemyGridCell.GetX(goInited.transform.position);
            var unitRuntimeInfo = runtimeUnitModel.GetRuntimeUnit(goInited.GetHashCode());
            if (bodySize == 1)
            {
                if (x == 0) unitRuntimeInfo.Threat = 3;
                else if (x == 1) unitRuntimeInfo.Threat = 2;
                else if (x == 2) unitRuntimeInfo.Threat = 1;
            }
            else if(bodySize == 2 || bodySize == 4)
            {
                if (x == 0) unitRuntimeInfo.Threat = 3;
                else if (x == 1) unitRuntimeInfo.Threat = 2;
            }
            else if (bodySize == 3)
            {
                if (x == 0) unitRuntimeInfo.Threat = 3;
            }
        }
        void InitHeroGoThread()
        {
            var x = gridSystem.HeroGridCell.GetX(goInited.transform.position);
            var unitRuntimeInfo = runtimeUnitModel.GetRuntimeUnit(goInited.GetHashCode());
            if (bodySize == 1)
            {
                if (x == 0) unitRuntimeInfo.Threat = 1;
                else if (x == 1) unitRuntimeInfo.Threat = 2;
                else if (x == 2) unitRuntimeInfo.Threat = 3;
            }
            else if (bodySize == 2 || bodySize == 4)
            {
                if (x == 0) unitRuntimeInfo.Threat = 2;
                else if (x == 1) unitRuntimeInfo.Threat = 3;
            }
            else if (bodySize == 3)
            {
                if (x == 0) unitRuntimeInfo.Threat = 3;
            }
        }
        private void Start()
        {
            runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
        }
        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }

        
    }
}