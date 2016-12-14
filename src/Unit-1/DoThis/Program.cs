using System;
using Akka.Actor;

namespace WinTail
{
	class Program
	{
		public static ActorSystem MyActorSystem;

		static void Main(string[] args)
		{
			// initialize MyActorSystem
			MyActorSystem = ActorSystem.Create("MyActorSystem");

			Props consoleWriterProps = Props.Create<ConsoleWriterActor>();
			IActorRef consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");

			Props consoleReaderProps = Props.Create<ConsoleReaderActor>(validationActor);
			IActorRef consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");

			// make tailCoordinatorActor
			Props tailCoordinatorProps = Props.Create(() => new TailCoordinatorActor());
			IActorRef tailCoordinatorActor = MyActorSystem.ActorOf(tailCoordinatorProps,
				"tailCoordinatorActor");

			// pass tailCoordinatorActor to fileValidatorActorProps (just adding one extra arg)
			Props fileValidatorActorProps = Props.Create(() =>
				new FileValidatorActor(consoleWriterActor, tailCoordinatorActor));
			IActorRef validationActor = MyActorSystem.ActorOf(fileValidatorActorProps,
				"validationActor");

			// tell console reader to begin
			consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

			// blocks the main thread from exiting until the actor system is shut down
			MyActorSystem.WhenTerminated.Wait();
		}
	}
}