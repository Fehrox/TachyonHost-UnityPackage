using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TachyonCommon;
using TachyonServerRPC;
using UnityEngine;

namespace TachyonServerCore {
    [RequireComponent(typeof(MainThreadReplyProcessor))]
    public class TachyonUnityHost : MonoBehaviour {

        public event Action<ClientConnection> OnClientConnected;
        public event Action<ClientConnection> OnClientDisconnected;
    
        private HostCore _host;

        private Queue<ClientConnection> _connections = new Queue<ClientConnection>();
        private Queue<ClientConnection> _disconnections = new Queue<ClientConnection>();
        public bool Started { get; set; }

        public void Initialize<TService>(
            ISerializer serializer, 
            params TService[] services
        ) where TService : class {
            var endPointMap = new EndPointMap(serializer);
            var replyProcessor = GetComponent<MainThreadReplyProcessor>();
            _host = new HostCore(endPointMap, replyProcessor);
            _host.OnConnected += (c) => _connections.Enqueue(c);
            _host.OnDisconnected += (c) => _disconnections.Enqueue(c);
            _host.OnStarted += () => Started = true;

            foreach (TService service in services) 
                _host.Bind(service);

            _host.Start();
        }

        // Synchronise threaded events with unity main thread.
        IEnumerator Start() {
            while (true) {
                
                if (_connections.Any()) {
                    var connection = _connections.Dequeue();
                    OnClientConnected?.Invoke(connection);
                }

                if (_disconnections.Any()) {
                    var disconnection = _disconnections.Dequeue();
                    OnClientDisconnected?.Invoke(disconnection);
                }

                yield return null;
            }
        }

    }
}