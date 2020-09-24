// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapFile.Services.Utils
{
    using System;
    using System.IO;
    using Omicron.SapFile.Entities.Models;

    /// <summary>
    /// The class for the services.
    /// </summary>
    public static class ServiceUtils
    {
        /// <summary>
        /// creates the result.
        /// </summary>
        /// <param name="success">if it was successful.</param>
        /// <param name="code">the code.</param>
        /// <param name="userError">the user error.</param>
        /// <param name="responseObj">the responseobj.</param>
        /// <param name="exceptionMessage">the exception message.</param>        
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage)
        {
            return new ResultModel
            {
                Success = success,
                Response = responseObj,
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Code = code
            };
        }

        /// <summary>
        /// Return a bin directory.
        /// </summary>
        /// <returns>Bin directory.</returns>
        public static string GetBinDirectory()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var root = Directory.GetCurrentDirectory();
            if (root.EndsWith("\\Debug"))
            {
                return root;
            }
            return Path.Combine(Directory.GetCurrentDirectory(), "bin");
        }

        /// <summary>
        /// Copy file with validations.
        /// </summary>
        /// <param name="src">Source.</param>
        /// <param name="dest">Destination.</param>
        public static void CopyFile(string src, string dest)
        {
            if (File.Exists(src))
            {
                var destPath = Path.GetDirectoryName(dest);

                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }

                if (File.Exists(dest))
                {
                    File.Delete(dest);
                }

                File.Copy(src, dest);
            }
        }
    }
}
