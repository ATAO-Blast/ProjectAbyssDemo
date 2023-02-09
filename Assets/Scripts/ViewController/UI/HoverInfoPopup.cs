using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace AbyssDemo
{
    public class HoverInfoPopup : MonoBehaviour,IController
    {
        [SerializeField] private GameObject popupCanvasObject = null;
        [SerializeField] private RectTransform popupObject = null;
        [SerializeField] private TextMeshProUGUI infoText = null;
        [SerializeField] private Vector3 offset = new Vector3(0, 5f, 0);
        [SerializeField] private float padding = 25f;

        private Canvas popupCanvas = null;

        // Start is called before the first frame update
        void Start()
        {
            popupCanvas = popupCanvasObject.GetComponent<Canvas>();
            TypeEventSystem.Global.Register<OnAtlasUIHover>(e =>
            {
                DisplayInfo(e.baseUnitSO, e.IsCanvasOn);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnUnitHover>(e =>
            {
                DisplayUnitInfo(e.unitHovering, e.IsCanvasOn);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            FollowCursor();
        }

        private void FollowCursor()
        {
            if (!popupCanvasObject.activeSelf) { return; }

            Vector3 newPos = Input.mousePosition + offset;
            newPos.z = 0f;

            float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + popupObject.rect.width * popupCanvas.scaleFactor / 2) - padding;
            if (rightEdgeToScreenEdgeDistance < 0)
            {
                newPos.x += rightEdgeToScreenEdgeDistance;
            }
            float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - popupObject.rect.width * popupCanvas.scaleFactor / 2) + padding;
            if (leftEdgeToScreenEdgeDistance > 0)
            {
                newPos.x += leftEdgeToScreenEdgeDistance;
            }
            float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + popupObject.rect.height * popupCanvas.scaleFactor) - padding;
            if (topEdgeToScreenEdgeDistance < 0)
            {
                newPos.y += topEdgeToScreenEdgeDistance;
            }

            popupObject.transform.position = newPos;
        }
        void DisplayInfo(BaseUnitSO baseUnitSO,bool canvasEnable)
        {
            if(baseUnitSO != null)
            {
                infoText.text = baseUnitSO.DisplaySOInfo();
            }

            popupCanvasObject.SetActive(canvasEnable);

            LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);//有的时候文本框不会改变大小，使用此方法强制刷新
        }
        void DisplayUnitInfo(RuntimeUnitInfo runtimeUnitInfo,bool canvasEnable)
        {
            if (runtimeUnitInfo != null)
            {
                var buffDic = this.GetSystem<ElementBuffSystem>().GetUnitBuffDic(runtimeUnitInfo);
                infoText.text = runtimeUnitInfo.DisplayInfo();
                if(buffDic != null)
                {
                    foreach (var item in buffDic.Keys)
                    {
                        infoText.text += "\n" + item.ToString();
                    }
                }
            }

            popupCanvasObject.SetActive(canvasEnable);

            LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);//有的时候文本框不会改变大小，使用此方法强制刷新
        }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }
    }
}