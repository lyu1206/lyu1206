using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Battlehub.RTEditor
{
    using Eos.Objects;
    using RTCommon;
    public class EosPropertyEditor : ComponentEditor
    {

        [SerializeField]
        private TextMeshProUGUI _category;
        private IEditorsMap m_editorsMap;
        private EosObjectBase _target;
        private List<InspectorComponent.InspectorSet> _properties;
        public void SetProperties(string category,List<InspectorComponent.InspectorSet> Properties,EosObjectBase target)
        {
            _category.text = category;
            _target = target;
            _properties = Properties;
        }
        protected override void Awake()
        {
            base.Awake();
            m_editorsMap = IOC.Resolve<IEditorsMap>();
        }
        protected override void BuildEditor(IComponentDescriptor componentDescriptor, PropertyDescriptor[] descriptors)
        {
            //            base.BuildEditor(componentDescriptor, descriptors);
            foreach (var it in _properties)
            {
                var propertyeditorobject = m_editorsMap.GetPropertyEditor(it.PropertyInfo.PropertyType);
                var propertyeditor = Instantiate(propertyeditorobject.GetComponent<PropertyEditor>()) as PropertyEditor;
                var propertydescriptor = new PropertyDescriptor(it.PropertyInfo.Name, _target, it.PropertyInfo);
                propertydescriptor.ValueChangedCallback = () =>
                {
                    Debug.Log($"value changed:{it.PropertyInfo.Name}");
                };
                InitEditor(propertyeditor, propertydescriptor);
                propertyeditor.transform.SetParent(EditorsPanel, false);
            }
        }
    }
}