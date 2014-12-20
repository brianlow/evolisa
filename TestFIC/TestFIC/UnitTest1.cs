using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFIC
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class UnitTest1
	{
		public UnitTest1()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		private double compare( Bitmap b1, Bitmap b2, Rectangle r )
		{
			BitmapData bd1 = b1.LockBits( r, ImageLockMode.ReadWrite, b1.PixelFormat );
			BitmapData bd2 = b2.LockBits( r, ImageLockMode.ReadWrite, b2.PixelFormat );
			double retVal = FIC.FastImageCompare.compare(
				bd1.Scan0,
				bd2.Scan0,
				r.Width * r.Height );
			b1.UnlockBits( bd1 );
			b2.UnlockBits( bd2 );
			return retVal;
		}

		[TestMethod]
		public void TestCorrectCalculation()
		{
			int width = 4000, height = 3000;
			Rectangle r = new Rectangle( 0, 0, width, height );

			using ( Bitmap
				bmpWhite = new Bitmap( width, height ),
				bmpBlack = new Bitmap( width, height ),
				bmpBlue = new Bitmap( width, height ) )
			{
				using ( Graphics g = Graphics.FromImage( bmpWhite ) )
					g.FillRectangle( Brushes.White, r );
				using ( Graphics g = Graphics.FromImage( bmpBlack ) )
					g.FillRectangle( Brushes.Black, r );
				using ( Graphics g = Graphics.FromImage( bmpBlue ) )
					g.FillRectangle( Brushes.DarkBlue, r );

				Assert.AreEqual(
					compare( bmpBlack, bmpWhite, r ),
					width * height * 3 * Math.Pow( 255, 2),
					"Failed to verify black <-> while" );
				Assert.AreEqual(
				  compare( bmpBlack, bmpBlue, r ),
					width * height * 1 * Math.Pow( Color.DarkBlue.B, 2 ),
				  "Failed to verify black <-> blue" );
				Assert.AreEqual(
					compare( bmpWhite, bmpBlue, r ),
					width * height * 2 * Math.Pow( 255, 2 ) +
					width * height * 1 * Math.Pow( Color.DarkBlue.B - Color.White.B, 2 ),
					"Failed to verify white <-> blue" );
			}
		}
	}
}
