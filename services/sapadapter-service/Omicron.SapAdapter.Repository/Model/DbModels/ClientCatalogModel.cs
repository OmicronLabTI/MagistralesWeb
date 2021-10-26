// <summary>
// <copyright file="ClientCatalogModel.cs" company="Axity">
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
    /// The asesor table.
    /// </summary>
    [Table("OCRD")]
    public class ClientCatalogModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Key]
        [Column("CardCode")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("E_Mail")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the doctor Name.
        /// </summary>
        /// <value>Doctor Name.</value>
        [Column("CardName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the group code.
        /// </summary>
        /// <value>Group code.</value>
        [Column("AliasName")]
        public string AliasName { get; set; }

        /// <summary>
        /// Gets or sets the slp for advisor.
        /// </summary>
        /// <value>Slpcode.</value>
        [Column("SlpCode")]
        public int SlpCode { get; set; }
    }
}
