using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using DAL;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ProAppWebApi
{
    public class SignalRChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> FromUsers = new ConcurrentDictionary<string, string>();         // <connectionId, userName>
        private static ConcurrentDictionary<string, string> ToUsers = new ConcurrentDictionary<string, string>();           // <userName, connectionId>
        private string userName = "";

        public override Task OnConnected()
        {
            DoConnect();
            Clients.All.broadcastMessage(new ChatMessage() { UserName = userName, Message = "I'm Online" });
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled) // Client explicitly closed the connection
            {
                string id = Context.ConnectionId;
                FromUsers.TryRemove(id, out userName);
                ToUsers.TryRemove(userName, out id);
                Clients.AllExcept(Context.ConnectionId).broadcastMessage(new ChatMessage() { UserName = userName, Message = "I'm Offline" });
            }
            else // Client timed out
            {
                // Do nothing here...
                // FromUsers.TryGetValue(Context.ConnectionId, out userName);            
                // Clients.AllExcept(Context.ConnectionId).broadcastMessage(new ChatMessage() { UserName = userName, Message = "I'm Offline By TimeOut"});                
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            DoConnect();
            Clients.All.broadcastMessage(new ChatMessage() { UserName = userName, Message = "I'm Online Again" });
            return base.OnReconnected();
        }

        private void DoConnect()
        {
            userName = Context.Request.Headers["User-Name"];
            if (userName == null || userName.Length == 0)
            {
                userName = Context.QueryString["User-Name"]; // for javascript clients
            }
            FromUsers.TryAdd(Context.ConnectionId, userName);
            String oldId; // for case: disconnected from Client
            ToUsers.TryRemove(userName, out oldId);
            ToUsers.TryAdd(userName, Context.ConnectionId);
        }

        public void Send(string message)
        {
            // Call the broadcastMessage method to update clients.            
            //string fromUser;
            //FromUsers.TryGetValue(Context.ConnectionId, out fromUser);
            Clients.AllExcept(Context.ConnectionId).broadcastMessage(new ChatMessage() { UserName = "self", Message = message });
        }

        public void SendChatMessage(string to, string message)
        {
            FromUsers.TryGetValue(Context.ConnectionId, out userName);
            string receiver_ConnectionId;
            ToUsers.TryGetValue(to, out receiver_ConnectionId);

            if (receiver_ConnectionId != null && receiver_ConnectionId.Length > 0)
            {
                Clients.Client(receiver_ConnectionId).broadcastMessage(new ChatMessage() { UserName = userName, Message = message });
            }
        }
        //[HubName("signalRChatHub")]
        //public class SignalRChatHub : Hub
        //{
        //    public override System.Threading.Tasks.Task OnConnected()
        //    {


        //        Clients.AllExcept(Context.ConnectionId).broadcastMessage(new ChatMessage() { UserName = "Server", Message = "I'm Online" });
        //        return base.OnConnected();
        //        //string clientId = Context.ConnectionId;
        //        //string data = clientId;
        //        //string count = "";
        //        //try
        //        //{
        //        //    using (ChatONv1Entities entities = new ChatONv1Entities())
        //        //    {
        //        //        count = entities.ConnectedUsers.Count().ToString();
        //        //    }
        //        //}
        //        //catch (Exception d)
        //        //{
        //        //    count = d.Message;
        //        //}
        //        //Clients.Caller.receiveMessage("ChatHub", data, count);
        //        //return base.OnConnected();
        //    }

        //    [HubMethodName("hubconnect")]
        //    public void Get_Connect(String userlogin, String userid, String connectionid)
        //    {
        //        string count = "";
        //        string msg = "";
        //        string list = "";
        //        try
        //        {
        //            using (ChatONv1Entities entities = new ChatONv1Entities())
        //            {
        //                //count = entities.ConnectedUsers.Count().ToString();
        //                ConnectedUser obj = new ConnectedUser();
        //                obj.Login = userlogin;
        //                obj.ConnID = connectionid;

        //                entities.ConnectedUsers.Add(obj);
        //                entities.SaveChanges();

        //                msg = "truly added to DB/Connected";
        //                //list = entities.ConnectedUsers.ToString();

        //            }

        //        }
        //        catch (Exception d)
        //        {
        //            msg = "DB Error " + d.Message;
        //        }
        //        var id = Context.ConnectionId;
        //        string[] Exceptional = new string[1];
        //        Exceptional[0] = id;
        //        Clients.Caller.receiveMessage("RU", msg, list);
        //        //Clients.AllExcept(Exceptional).receiveMessage("NewConnection", userlogin + " " + id, count);
        //    }

        //    //[HubMethodName("privatemessage")]
        //    public void Send_PrivateMessage(String msgFrom, String msg, String touserlogin)
        //    {
        //        var id = Context.ConnectionId;

        //        try
        //        {
        //            using (ChatONv1Entities entities = new ChatONv1Entities())
        //            {
        //                Message message = new Message(); message.isRead = true; message.Message1 = msg; message.Recipient = touserlogin; message.Sender = msgFrom;
        //                entities.Messages.Add(message);
        //                entities.SaveChanges();

        //                var v = entities.ConnectedUsers.Where(e => e.Login == touserlogin).FirstOrDefault();
        //                //if (v == null)
        //                //return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Message with id " + id.ToString() + " couldn't be found!!!");
        //                String touserid = v.ConnID;

        //                Clients.Caller.receiveMessage(msgFrom, msg, touserid);
        //                Clients.Client(touserid).receiveMessage(msgFrom, msg, id);
        //                //return Request.CreateResponse(HttpStatusCode.OK, v);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
        //        }

        //    }

        //    public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        //    {
        //        string count = "";
        //        string msg = "";
        //        string clientId = Context.ConnectionId;

        //        try
        //        {
        //            using (ChatONv1Entities entities = new ChatONv1Entities())
        //            {
        //                var v = entities.ConnectedUsers.Where(e => e.ConnID == clientId).FirstOrDefault();
        //                if (v != null)
        //                {
        //                    entities.ConnectedUsers.Remove(v);
        //                    entities.SaveChanges();
        //                }

        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
        //        }
        //        try
        //        {
        //            using (ChatONv1Entities entities = new ChatONv1Entities())
        //            {
        //                count = entities.ConnectedUsers.Count().ToString();
        //            }
        //        }
        //        catch (Exception d)
        //        {
        //            msg = "DB Error " + d.Message;
        //        }
        //        string[] Exceptional = new string[1];
        //        Exceptional[0] = clientId;
        //        Clients.AllExcept(Exceptional).receiveMessage("NewConnection", clientId + " leave", count);
        //        return base.OnDisconnected(stopCalled);
        //    }
        //}

        internal class ChatMessage
        {
            public ChatMessage()
            {
            }

            public string Message { get; set; }
            public object UserName { get; set; }
        }
    }
}