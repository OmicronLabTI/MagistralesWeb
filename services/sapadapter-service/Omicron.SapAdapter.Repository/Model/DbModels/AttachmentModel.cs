// <summary>
// <copyright file="AttachmentModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.DbModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class for ATC1.
    /// </summary>
    [Table("ATC1")]
    public class AttachmentModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Key]
        [Column("AbsEntry")]
        public int AbsEntry { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Key]
        [Column("Line")]
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("srcPath")]
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("trgtPath")]
        public string TargetPath { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("FileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("FileExt")]
        public string FileExt { get; set; }

        /// <summary>
        /// Gets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is sales order.
        [NotMapped]
        public string CompletePath => $"{this.TargetPath}/{this.FileName}.{this.FileExt}";
    }
}
