using System;
using System.Threading;
using UnityEngine;

namespace Async {
	public class AsyncManager : MonoBehaviour {
		private readonly CancellationTokenSource _tokenSource = new ();
		
		public static CancellationToken Token => _instance._tokenSource.Token;
		
		private static AsyncManager _instance;

		private void Awake() {
			_instance = this;
		}

		private void OnDestroy()
		{
			_tokenSource.Cancel();
		}
	}
}