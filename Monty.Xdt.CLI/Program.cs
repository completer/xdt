using System;
using System.Xml.Linq;

namespace Monty.Xdt.CLI
{
	/// <summary>
	/// Utility that transforms config files on the command line.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Gets or sets the source file.
		/// </summary>
		/// <value>The source file.</value>
		public string SourceFile { get; set;  }

		/// <summary>
		/// Gets or sets the transform file.
		/// </summary>
		/// <value>The transform file.</value>
		public string TransformFile { get; set; }

		/// <summary>
		/// Gets or sets the destination file.
		/// </summary>
		/// <value>The destination file.</value>
		public string DestinationFile { get; set; }

		/// <summary>
		/// Transforms the source file using the transform file and writes the output into
		/// destination file.
		/// </summary>
		protected void Run()
		{
			var sourceDocument = XDocument.Load(SourceFile);
			var transformDocument = XDocument.Load(TransformFile);

			var xdtTransformer = new XdtTransformer();

			var destinationDocument = xdtTransformer.Transform(sourceDocument, transformDocument);

			destinationDocument.Save(DestinationFile);
		}

		public static void Main(string[] args)
		{
			Program program = new Program
				{
					SourceFile = args[0],
					TransformFile = args[1],
					DestinationFile = args[2]
				};
			program.Run();
		}
	}
}
