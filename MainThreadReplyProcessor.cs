using System;
using System.Collections.Generic;
using System.Linq;
using TachyonServerRPC;
using UnityEngine;

namespace TachyonServerCore {
    public class MainThreadReplyProcessor : MonoBehaviour, IReplyProcessor {
        
        private readonly Queue<Action> _replies = new Queue<Action>();
        
        public void ProcessReply(Action reply) {
            _replies.Enqueue(reply);
        }

        private void Update() {
            if (_replies.Any()) {
                var reply = _replies.Dequeue();
                reply?.Invoke();
            }
        }
    }
}