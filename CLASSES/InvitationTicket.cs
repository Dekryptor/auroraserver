namespace AuroraServer.CLASSES
{
	internal class InvitationTicket
	{
		internal string ID;
		internal Client Sender;
		internal Client Receiver;
		internal string GroupId;
		internal bool IsFollow;
		internal byte Result = byte.MaxValue;

		internal InvitationTicket(Client Sender, Client Receiver)
		{
			ID = Sender.Player.Random.Next(1, int.MaxValue).ToString();
			this.Sender = Sender;
			this.Receiver = Receiver;
		}
	}
}
