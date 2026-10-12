using System;
using System.Reflection;
using LightweightGame.Runtime;
using LightweightGame.Interaction;
using UnityEngine;
using UnityEngine.UI;

namespace LightweightGame.Editor
{
    public static class NpcInteractionTests
    {
        public static void Run()
        {
            Type npcType = Type.GetType("LightweightGame.Runtime.NpcInteractable, Assembly-CSharp");
            if (npcType == null) throw new InvalidOperationException("NPC interaction component missing");
            MethodInfo configure = npcType.GetMethod("Configure", new[] { typeof(string), typeof(string) });
            if (configure == null) throw new InvalidOperationException("NPC interaction configuration missing");
            if (typeof(InteractionPromptUI).GetMethod("ShowMessage") == null)
                throw new InvalidOperationException("Dialogue message display missing");
            MethodInfo clearMessage = typeof(InteractionPromptUI).GetMethod("ClearMessage");
            if (clearMessage == null) throw new InvalidOperationException("Dialogue message clearing missing");

            GameObject actor = new GameObject("NpcInteractionTestActor");
            GameObject npc = new GameObject("NpcInteractionTestNpc");
            GameObject canvas = new GameObject("NpcInteractionTestCanvas", typeof(RectTransform), typeof(Canvas));
            try
            {
                GameObject buttonObject = new GameObject("PromptButton", typeof(RectTransform), typeof(Image), typeof(Button));
                buttonObject.transform.SetParent(canvas.transform, false);
                Button button = buttonObject.GetComponent<Button>();
                GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
                labelObject.transform.SetParent(buttonObject.transform, false);
                Text label = labelObject.GetComponent<Text>();
                label.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
                InteractionPromptUI prompt = buttonObject.AddComponent<InteractionPromptUI>();
                prompt.Initialize(button, label);

                InteractionController receiver = actor.AddComponent<InteractionController>();
                receiver.enabled = false;
                typeof(InteractionController).GetField("prompt", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(receiver, prompt);
                Component component = npc.AddComponent(npcType);
                configure.Invoke(component, new object[] { "学生", "下节课去操场吧。" });
                Interactable interactable = npc.GetComponent<Interactable>();
                if (interactable == null || !interactable.CanInteract(actor.transform))
                    throw new InvalidOperationException("Configured NPC cannot be interacted with");
                if (interactable.Label != "交谈")
                    throw new InvalidOperationException("NPC prompt is not localized as a conversation");
                interactable.Activate(actor.transform);
                Transform messageObject = canvas.transform.Find("DialogueMessage");
                Text message = messageObject != null ? messageObject.GetComponent<Text>() : null;
                if (message == null || !message.gameObject.activeSelf || message.text != "学生：下节课去操场吧。")
                    throw new InvalidOperationException("NPC greeting was not displayed");
                clearMessage.Invoke(prompt, null);
                if (message.gameObject.activeSelf) throw new InvalidOperationException("NPC greeting was not cleared");
                Debug.Log("NPC_INTERACTION_PASS localized prompt and greeting");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(canvas);
                UnityEngine.Object.DestroyImmediate(npc);
                UnityEngine.Object.DestroyImmediate(actor);
            }
        }
    }
}
