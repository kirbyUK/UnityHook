using Hooks;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Hooks
{
	[RuntimeHook]
	class PacketDumpHook
	{
		private bool read_reentrant;
		private bool send_reentrant;
		private Stopwatch time = new Stopwatch();

		public PacketDumpHook()
		{
			HookRegistry.Register(OnCall);
			read_reentrant = false;
			send_reentrant = false;
			time.Start();
		}

		private void InitDynamicTypes() { }

		public static string[] GetExpectedMethods()
		{
			return new string[] {
				"dwd.core.wargServer.raw.RawReader::Read",
				"dwd.core.wargServer.WargSocket::Write",
//				"dwd.core.wargServer.WargSocket::Connect"
			};
		}

		private dwd.core.wargServer.raw.RawMessage result;

		void DumpPacket(dwd.core.wargServer.raw.RawMessage packet)
		{
			using (BinaryWriter b = new BinaryWriter(
				File.Open(
					String.Format(
						@"C:\Users\Alex\Documents\code\tcgo\network\packets\packet{0}-received.bin",
						time.ElapsedTicks
					),
					FileMode.Create)
				)
			)
			{
				b.Write(packet.Body);
			}
		}

		object OnCall(string typeName, string methodName, object thisObj, object[] args, IntPtr[] refArgs, int[] refIdxMatch)
		{
			if (methodName == "Read")
			{
				if (read_reentrant)
				{
					read_reentrant = false;
					return null;
				}
				read_reentrant = true;
				var reader = (dwd.core.wargServer.raw.RawReader)thisObj;
				result = reader.Read();
				DumpPacket(result);
				return result;
			}
			else if (methodName == "Write")
			{
				var socket = (dwd.core.wargServer.WargSocket)thisObj;
				object message = args[0];
				var flag = (dwd.core.wargServer.WargSocketLogConfiguration)typeof(dwd.core.wargServer.WargSocket).GetField("logConfiguration", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(socket);
				var attr = (dwd.core.wargServer.raw.RawWriter)typeof(dwd.core.wargServer.WargSocket).GetField("rawWriter", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(socket);
				System.IO.File.AppendAllText(
					String.Format(@"C:\Users\Alex\Documents\code\tcgo\network\packets\packet{0}-sent.bin", time.ElapsedTicks),
					contents: $"{System.Text.Encoding.UTF8.GetString(attr.GetMessageBytes(message, flag))}"
				);
			}
/*
			else if (methodName == "Connect")
			{
				if (send_reentrant)
				{
					send_reentrant = false;
					return null;
				}
				send_reentrant = true;
				typeof(dwd.core.wargServer.WargSocket).GetField("useSSL", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(thisObj, false);
			}
*/
			return null;
		}
	}
}
