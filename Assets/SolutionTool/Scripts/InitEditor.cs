using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.RTCommon
{
    using RTEditor;

    [DefaultExecutionOrder(-100)]
    public class InitEditor : MonoBehaviour, IRTEState
    {
        public event Action<object> Created;
        public event Action<object> Destroyed;

        public bool IsCreated
        {
            get { return m_editor != null; }
        }

        [SerializeField]
        private RTEBase m_editorPrefab = null;
        [SerializeField]
        private Splash m_splashPrefab = null;

        private RTEBase m_editor;

        private void Awake()
        {
            IOC.RegisterFallback<IRTEState>(this);
//            IOC.Register<IEditorsMapCreator>(new EosEditorsMapCreator());

            m_editor = (RTEBase)FindObjectOfType(m_editorPrefab.GetType());
            if (m_editor != null)
            {
                if (m_editor.IsOpened)
                {
                    m_editor.IsOpenedChanged += OnIsOpenedChanged;
                    gameObject.SetActive(false);
                }
            }
        }
        private void Start()
        {
            OnOpen();
        }
        private void OnDestroy()
        {
            IOC.UnregisterFallback<IRTEState>(this);
            if (m_editor != null)
            {
                m_editor.IsOpenedChanged -= OnIsOpenedChanged;
            }
        }

        private void OnOpen()
        {
            Debug.Log("OnOpen");
            if (m_splashPrefab != null)
            {
                Instantiate(m_splashPrefab).Show(() => InstantiateRuntimeEditor());
            }
            else
            {
                InstantiateRuntimeEditor();
            }
        }

        private void InstantiateRuntimeEditor()
        {
            m_editorPrefab.gameObject.SetActive(true);
            m_editor = m_editorPrefab;// Instantiate(m_editorPrefab);
            m_editor.name = "RuntimeEditor";
            m_editor.IsOpenedChanged += OnIsOpenedChanged;
            m_editor.transform.SetAsFirstSibling();
            if (Created != null)
            {
                Created(m_editor);
            }
            gameObject.SetActive(false);
        }

        private void OnIsOpenedChanged()
        {
            if (m_editor != null)
            {
                if (!m_editor.IsOpened)
                {
                    m_editor.IsOpenedChanged -= OnIsOpenedChanged;

                    if (this != null)
                    {
                        gameObject.SetActive(true);
                    }

                    Destroy(m_editor);

                    if (Destroyed != null)
                    {
                        Destroyed(m_editor);
                    }
                }
            }
        }
    }
}