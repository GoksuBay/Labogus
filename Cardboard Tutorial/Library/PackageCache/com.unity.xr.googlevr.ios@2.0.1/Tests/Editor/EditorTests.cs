using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UnityEditor.XR.GVR.iOS
{
    /// <summary>
    /// This class provides tests for the package while in the Editor.
    /// </summary>
    /// <remarks>
    /// Packages require XmlDoc documentation for ALL Package APIs.
    /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/xml-documentation-comments
    /// </remarks>
    public class GVRiOSTests
    {
        /// <summary>
        /// Checks if plugins from the package have been imported.
        /// </summary>
        [Test]
        public void EmptyTest()
        {
            Assert.IsTrue(true);
        }
    }
}