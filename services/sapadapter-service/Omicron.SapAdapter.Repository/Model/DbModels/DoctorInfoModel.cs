// <summary>
// <copyright file="DoctorInfoModel.cs" company="Axity">
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
    /// Doctor infor table.
    /// </summary>
    [Table("CRD1")]
    public class DoctorInfoModel
    {
        /// <summary>
        /// Gets or sets the info nickname.
        /// </summary>
        /// <value>Nickname.</value>
        [Key]
        [Column("Address")]
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the doctor card code.
        /// </summary>
        /// <value>CardCode.</value>
        [Column("CardCode")]
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <value>Street.</value>
        [Column("Address2")]
        public string Address2 { get; set; }
    }
}
