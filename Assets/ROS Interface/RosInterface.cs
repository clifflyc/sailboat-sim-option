using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class RosInterface : MonoBehaviour {

	//----------- for handling subscriptions -------//

	public abstract class _MessageEmitter {
		public abstract void ReceiveMessage (string message);
	}

	public class MessageEmitter<T> : _MessageEmitter {
		public delegate void MessageHandler (T message);

		public event MessageHandler MessageReceived;

		//for json utility parsing
		[System.Serializable]
		class MessageContainer {
			public T msg;

			public MessageContainer (T msg) {
				this.msg = msg;
			}
		}

		public override void ReceiveMessage (string rosMessage) {
			if (MessageReceived != null) {
				MessageContainer parsedMessage = JsonUtility.FromJson<MessageContainer> (rosMessage);
				MessageReceived.Invoke (parsedMessage.msg);
			}
		}
	}

	static Dictionary<string, _MessageEmitter> subscriptions = new Dictionary<string, _MessageEmitter> ();

	//------------------//

	[System.Serializable]
	class RosCommand {
		public string op;
		public string topic;
		public string type;

		public RosCommand (string op, string topic, string type) {
			this.op = op;
			this.topic = topic;
			this.type = type;
		}
	}

	[System.Serializable]
	class RosPubCommand <T> {
		public string op;
		public string topic;
		public T msg;

		public RosPubCommand (string op, string topic, T msg) {
			this.op = op;
			this.topic = topic;
			this.msg = msg;
		}
	}


	//----------------------//

	static RosInterface instance;
	static WebSocket ws;


	void Awake () {
		if (!instance) {
			instance = this;
		} else {
			Debug.LogWarning ("Please only have one instance of RosInterface script.");
		}
	}

	//----------------------//

	public static void Connect (string url) {
		Debug.Log ("Connecting to " + url);
		if (ws != null) {
			ws.Close ();
		}
		ws = new WebSocket (url);
		ws.OnOpen += OnWsOpen;
		ws.OnMessage += OnWsMessage;
		ws.Connect ();
		//TODO handle dc
	}

	public static void Disconnect () {
		if (ws != null) {
			ws.Close ();
			Debug.Log ("Websocket closed");
		}
	}

	public static MessageEmitter<T> Subscribe<T> (string topic, string type) {
		Send (JsonUtility.ToJson (new RosCommand  ("subscribe", topic, type)));
		MessageEmitter<T> messageEmitter = new MessageEmitter<T> ();
		subscriptions [topic] = messageEmitter;
		return messageEmitter;
	}

	public static void Advertise (string topic, string type) {
		Send (JsonUtility.ToJson (new RosCommand ("advertise", topic, type)));
	}

	public static void Publish<T> (string topic, T message) {
		Send (JsonUtility.ToJson (new RosPubCommand<T> ("publish", topic, message)));
	}

	static void Send (string message) {
		//TODO wait for connect
		if (ws != null && ws.IsAlive) {
			ws.Send (message);
		}
	}

	static void OnWsOpen (object sender, System.EventArgs e) {
		Debug.Log ("Websocket open");
	}

	static void OnWsMessage (object sender, MessageEventArgs e) {
		RosCommand rosMessage = JsonUtility.FromJson<RosCommand> (e.Data);
		if (subscriptions.ContainsKey (rosMessage.topic)) {
			subscriptions [rosMessage.topic].ReceiveMessage (e.Data);
		}

	}

	//--------------------//

	void OnApplcationQuit () {
		Disconnect ();
	}

}
