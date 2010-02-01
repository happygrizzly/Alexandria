using System.Linq;
using Alexandria.Backend.Model;
using Alexandria.Messages;
using NHibernate;
using Rhino.ServiceBus;

namespace Alexandria.Backend.Consumers
{
	public class MyBooksRequestConsumer : ConsumerOf<MyBooksRequest>
	{
		private readonly ISession session;
		private readonly IServiceBus bus;

		public MyBooksRequestConsumer(ISession session, IServiceBus bus)
		{
			this.session = session;
			this.bus = bus;
		}

		public void Consume(MyBooksRequest message)
		{
			var books = session.CreateQuery("select b from User u join u.CurrentlyReading b join fetch b.Authors where u.Id = :id")
				.SetParameter("id", message.UserId)
				.List<Book>();

			bus.Reply(new MyBooksResponse
			{
				Books = books.Select(book => new BookDTO
				{
					Id = book.Id,
					ImageUrl = book.ImageUrl,
					Name = book.Name,
					Authors = book.Authors.Select(x => x.Name).ToArray()
				}).ToArray()
			});
		}
	}
}