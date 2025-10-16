// <summary>
// <copyright file="DoctorDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Doctor
{
    /// <summary>
    /// Class DoctorDto.
    /// </summary>
    public class DoctorDto
    {
        /// <summary>
        /// Gets or sets DoctorId.
        /// </summary>
        /// <value>The DoctorId.</value>
        [JsonProperty("CardCode")]
        public string DoctorId { get; set; }

        /// <summary>
        /// Gets or sets ListNum.
        /// </summary>
        /// <value>The ListNum.</value>
        [JsonProperty("PriceListNum")]
        public int ListNum { get; set; }

        /// <summary>
        /// Gets or sets CardName.
        /// </summary>
        /// <value>The CardName.</value>
        [JsonProperty("CardName")]
        public string CardName { get; set; }

        /// <summary>
        /// Gets or sets CardType.
        /// </summary>
        /// <value>The CardType.</value>
        [JsonProperty("CardType")]
        public string CardType { get; set; }

        /// <summary>
        /// Gets or sets Address.
        /// </summary>
        /// <value>The Address.</value>
        [JsonProperty("Address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        /// <value>The ZipCode.</value>
        [JsonProperty("ZipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets MailAddress.
        /// </summary>
        /// <value>The MailAddress.</value>
        [JsonProperty("MailAddress")]
        public string MailAddress { get; set; }

        /// <summary>
        /// Gets or sets MailZipCode.
        /// </summary>
        /// <value>The MailZipCode.</value>
        [JsonProperty("MailZipCode")]
        public string MailZipCode { get; set; }

        /// <summary>
        /// Gets or sets Phone1.
        /// </summary>
        /// <value>The Phone1.</value>
        [JsonProperty("Phone1")]
        public string Phone1 { get; set; }

        /// <summary>
        /// Gets or sets Phone2.
        /// </summary>
        /// <value>The Phone2.</value>
        [JsonProperty("Phone2")]
        public string Phone2 { get; set; }

        /// <summary>
        /// Gets or sets ContactPerson.
        /// </summary>
        /// <value>The ContactPerson.</value>
        [JsonProperty("ContactPerson")]
        public string ContactPerson { get; set; }

        /// <summary>
        /// Gets or sets Notes.
        /// </summary>
        /// <value>The Notes.</value>
        [JsonProperty("Notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets TaxID.
        /// </summary>
        /// <value>The TaxID.</value>
        [JsonProperty("FederalTaxID")]
        public string TaxID { get; set; }

        /// <summary>
        /// Gets or sets SalesPersonCode.
        /// </summary>
        /// <value>The SalesPersonCode.</value>
        [JsonProperty("SalesPersonCode")]
        public int SalesPersonCode { get; set; }

        /// <summary>
        /// Gets or sets Currency.
        /// </summary>
        /// <value>The Currency.</value>
        [JsonProperty("Currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets City.
        /// </summary>
        /// <value>The City.</value>
        [JsonProperty("City")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets Country.
        /// </summary>
        /// <value>The Country.</value>
        [JsonProperty("Country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets MailCity.
        /// </summary>
        /// <value>The MailCity.</value>
        [JsonProperty("MailCity")]
        public string MailCity { get; set; }

        /// <summary>
        /// Gets or sets MailCountry.
        /// </summary>
        /// <value>The MailCountry.</value>
        [JsonProperty("MailCountry")]
        public string MailCountry { get; set; }

        /// <summary>
        /// Gets or sets EmailAddress.
        /// </summary>
        /// <value>The EmailAddress.</value>
        [JsonProperty("EmailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets CardForeignName.
        /// </summary>
        /// <value>The CardForeignName.</value>
        [JsonProperty("CardForeignName")]
        public string CardForeignName { get; set; }

        /// <summary>
        /// Gets or sets Block.
        /// </summary>
        /// <value>The Block.</value>
        [JsonProperty("Block")]
        public string Block { get; set; }

        /// <summary>
        /// Gets or sets BillToState.
        /// </summary>
        /// <value>The BillToState.</value>
        [JsonProperty("BillToState")]
        public string BillToState { get; set; }

        /// <summary>
        /// Gets or sets ShipToState.
        /// </summary>
        /// <value>The ShipToState.</value>
        [JsonProperty("ShipToState")]
        public string ShipToState { get; set; }

        /// <summary>
        /// Gets or sets ShipToDefault.
        /// </summary>
        /// <value>The ShipToDefault.</value>
        [JsonProperty("ShipToDefault")]
        public string ShipToDefault { get; set; }

        /// <summary>
        /// Gets or sets ShipToDefault.
        /// </summary>
        /// <value>The ShipToDefault.</value>
        [JsonProperty("Valid")]
        public string Valid { get; set; }

        /// <summary>
        /// Gets or sets GroupCode.
        /// </summary>
        /// <value>The GroupCode.</value>
        [JsonProperty("GroupCode")]
        public int GroupCode { get; set; }

        /// <summary>
        /// Gets or sets PayTermsGrpCode.
        /// </summary>
        /// <value>The PayTermsGrpCode.</value>
        [JsonProperty("PayTermsGrpCode")]
        public int? PayTermsGrpCode { get; set; }

        /// <summary>
        /// Gets or sets CreditLimit.
        /// </summary>
        /// <value>The CreditLimit.</value>
        [JsonProperty("CreditLimit")]
        public double? CreditLimit { get; set; }

        /// <summary>
        /// Gets or sets MaxCommitment.
        /// </summary>
        /// <value>The MaxCommitment.</value>
        [JsonProperty("MaxCommitment")]
        public double? MaxCommitment { get; set; }

        /// <summary>
        /// Gets or sets DiscountPercent.
        /// </summary>
        /// <value>The DiscountPercent.</value>
        [JsonProperty("DiscountPercent")]
        public double? DiscountPercent { get; set; }

        /// <summary>
        /// Gets or sets VatLiable.
        /// </summary>
        /// <value>The VatLiable.</value>
        [JsonProperty("VatLiable")]
        public string VatLiable { get; set; }

        /// <summary>
        /// Gets or sets DeductibleAtSource.
        /// </summary>
        /// <value>The DeductibleAtSource.</value>
        [JsonProperty("DeductibleAtSource")]
        public string DeductibleAtSource { get; set; }

        /// <summary>
        /// Gets or sets DeductionPercent.
        /// </summary>
        /// <value>The DeductionPercent.</value>
        [JsonProperty("DeductionPercent")]
        public double? DeductionPercent { get; set; }

        /// <summary>
        /// Gets or sets the current account balance.
        /// </summary>
        /// <value>The current account balance.</value>
        [JsonProperty("CurrentAccountBalance")]
        public double? CurrentAccountBalance { get; set; }

        /// <summary>
        /// Gets or sets the open delivery notes balance.
        /// </summary>
        /// <value>The open delivery notes balance.</value>
        [JsonProperty("OpenDeliveryNotesBalance")]
        public double? OpenDeliveryNotesBalance { get; set; }

        /// <summary>
        /// Gets or sets the open orders balance.
        /// </summary>
        /// <value>The open orders balance.</value>
        [JsonProperty("OpenOrdersBalance")]
        public double? OpenOrdersBalance { get; set; }

        /// <summary>
        /// Gets or sets the open checks balance.
        /// </summary>
        /// <value>The open checks balance.</value>
        [JsonProperty("OpenChecksBalance")]
        public double? OpenChecksBalance { get; set; }

        /// <summary>
        /// Gets or sets the interest rate percent.
        /// </summary>
        /// <value>The interest rate percent.</value>
        [JsonProperty("IntrestRatePercent")]
        public double? IntrestRatePercent { get; set; }

        /// <summary>
        /// Gets or sets the commission percent.
        /// </summary>
        /// <value>The commission percent.</value>
        [JsonProperty("CommissionPercent")]
        public double? CommissionPercent { get; set; }

        /// <summary>
        /// Gets or sets the minimum interest.
        /// </summary>
        /// <value>The minimum interest.</value>
        [JsonProperty("MinIntrest")]
        public double? MinIntrest { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of exemption.
        /// </summary>
        /// <value>The maximum amount of exemption.</value>
        [JsonProperty("MaxAmountOfExemption")]
        public double? MaxAmountOfExemption { get; set; }

        /// <summary>
        /// Gets or sets the commission group code.
        /// </summary>
        /// <value>The commission group code.</value>
        [JsonProperty("CommissionGroupCode")]
        public int? CommissionGroupCode { get; set; }

        /// <summary>
        /// Gets or sets the credit card code.
        /// </summary>
        /// <value>The credit card code.</value>
        [JsonProperty("CreditCardCode")]
        public int? CreditCardCode { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        [JsonProperty("Priority")]
        public int? Priority { get; set; }

        /// <summary>
        /// Gets or sets the payment block description.
        /// </summary>
        /// <value>The payment block description.</value>
        [JsonProperty("PaymentBlockDescription")]
        public int? PaymentBlockDescription { get; set; }

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        [JsonProperty("LanguageCode")]
        public int? LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the withholding tax deduction group.
        /// </summary>
        /// <value>The withholding tax deduction group.</value>
        [JsonProperty("WithholdingTaxDeductionGroup")]
        public int? WithholdingTaxDeductionGroup { get; set; }

        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>The series.</value>
        [JsonProperty("Series")]
        public int? Series { get; set; }

        /// <summary>
        /// Gets or sets the owner code.
        /// </summary>
        /// <value>The owner code.</value>
        [JsonProperty("OwnerCode")]
        public int? OwnerCode { get; set; }

        /// <summary>
        /// Gets or sets the data version.
        /// </summary>
        /// <value>The data version.</value>
        [JsonProperty("DataVersion")]
        public int? DataVersion { get; set; }

        /// <summary>
        /// Gets or sets the default bank code.
        /// </summary>
        /// <value>The default bank code.</value>
        [JsonProperty("DefaultBankCode")]
        public string DefaultBankCode { get; set; }

        /// <summary>
        /// Gets or sets the father type.
        /// </summary>
        /// <value>The father type.</value>
        [JsonProperty("FatherType")]
        public string FatherType { get; set; }

        /// <summary>
        /// Gets or sets the credit card number.
        /// </summary>
        /// <value>The credit card number.</value>
        [JsonProperty("CreditCardNum")]
        public string CreditCardNum { get; set; }

        /// <summary>
        /// Gets or sets the debitor account.
        /// </summary>
        /// <value>The debitor account.</value>
        [JsonProperty("DebitorAccount")]
        public string DebitorAccount { get; set; }

        /// <summary>
        /// Gets or sets the frozen status.
        /// </summary>
        /// <value>The frozen status.</value>
        [JsonProperty("Frozen")]
        public string Frozen { get; set; }

        /// <summary>
        /// Gets or sets the payment method code.
        /// </summary>
        /// <value>The payment method code.</value>
        [JsonProperty("PeymentMethodCode")]
        public string PeymentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets the back order status.
        /// </summary>
        /// <value>The back order status.</value>
        [JsonProperty("BackOrder")]
        public string BackOrder { get; set; }

        /// <summary>
        /// Gets or sets the partial delivery status.
        /// </summary>
        /// <value>The partial delivery status.</value>
        [JsonProperty("PartialDelivery")]
        public string PartialDelivery { get; set; }

        /// <summary>
        /// Gets or sets the block dunning status.
        /// </summary>
        /// <value>The block dunning status.</value>
        [JsonProperty("BlockDunning")]
        public string BlockDunning { get; set; }

        /// <summary>
        /// Gets or sets the house bank country.
        /// </summary>
        /// <value>The house bank country.</value>
        [JsonProperty("HouseBankCountry")]
        public string HouseBankCountry { get; set; }

        /// <summary>
        /// Gets or sets the collection authorization status.
        /// </summary>
        /// <value>The collection authorization status.</value>
        [JsonProperty("CollectionAuthorization")]
        public string CollectionAuthorization { get; set; }

        /// <summary>
        /// Gets or sets the single payment status.
        /// </summary>
        /// <value>The single payment status.</value>
        [JsonProperty("SinglePayment")]
        public string SinglePayment { get; set; }

        /// <summary>
        /// Gets or sets the payment block status.
        /// </summary>
        /// <value>The payment block status.</value>
        [JsonProperty("PaymentBlock")]
        public string PaymentBlock { get; set; }

        /// <summary>
        /// Gets or sets the deferred tax status.
        /// </summary>
        /// <value>The deferred tax status.</value>
        [JsonProperty("DeferredTax")]
        public string DeferredTax { get; set; }

        /// <summary>
        /// Gets or sets the equalization status.
        /// </summary>
        /// <value>The equalization status.</value>
        [JsonProperty("Equalization")]
        public string Equalization { get; set; }

        /// <summary>
        /// Gets or sets the subject to withholding tax status.
        /// </summary>
        /// <value>The subject to withholding tax status.</value>
        [JsonProperty("SubjectToWithholdingTax")]
        public string SubjectToWithholdingTax { get; set; }

        /// <summary>
        /// Gets or sets the accrual criteria status.
        /// </summary>
        /// <value>The accrual criteria status.</value>
        [JsonProperty("AccrualCriteria")]
        public string AccrualCriteria { get; set; }

        /// <summary>
        /// Gets or sets the down payment clearing account.
        /// </summary>
        /// <value>The down payment clearing account.</value>
        [JsonProperty("DownPaymentClearAct")]
        public string DownPaymentClearAct { get; set; }

        /// <summary>
        /// Gets or sets the bill-to default.
        /// </summary>
        /// <value>The bill-to default.</value>
        [JsonProperty("BilltoDefault")]
        public string BilltoDefault { get; set; }

        /// <summary>
        /// Gets or sets the ship-to building, floor, and room.
        /// </summary>
        /// <value>The ship-to building, floor, and room.</value>
        [JsonProperty("ShipToBuildingFloorRoom")]
        public string ShipToBuildingFloorRoom { get; set; }

        /// <summary>
        /// Gets or sets the company private status.
        /// </summary>
        /// <value>The company private status.</value>
        [JsonProperty("CompanyPrivate")]
        public string CompanyPrivate { get; set; }

        /// <summary>
        /// Gets or sets the tax rounding rule.
        /// </summary>
        /// <value>The tax rounding rule.</value>
        [JsonProperty("TaxRoundingRule")]
        public string TaxRoundingRule { get; set; }

        /// <summary>
        /// Gets or sets the discount base object.
        /// </summary>
        /// <value>The discount base object.</value>
        [JsonProperty("DiscountBaseObject")]
        public string DiscountBaseObject { get; set; }

        /// <summary>
        /// Gets or sets the discount relations.
        /// </summary>
        /// <value>The discount relations.</value>
        [JsonProperty("DiscountRelations")]
        public string DiscountRelations { get; set; }

        /// <summary>
        /// Gets or sets the type report.
        /// </summary>
        /// <value>The type report.</value>
        [JsonProperty("TypeReport")]
        public string TypeReport { get; set; }

        /// <summary>
        /// Gets or sets the threshold overlook status.
        /// </summary>
        /// <value>The threshold overlook status.</value>
        [JsonProperty("ThresholdOverlook")]
        public string ThresholdOverlook { get; set; }

        /// <summary>
        /// Gets or sets the surcharge overlook status.
        /// </summary>
        /// <value>The surcharge overlook status.</value>
        [JsonProperty("SurchargeOverlook")]
        public string SurchargeOverlook { get; set; }

        /// <summary>
        /// Gets or sets the operation code 347.
        /// </summary>
        /// <value>The operation code 347.</value>
        [JsonProperty("OperationCode347")]
        public string OperationCode347 { get; set; }

        /// <summary>
        /// Gets or sets the insurance operation 347 status.
        /// </summary>
        /// <value>The insurance operation 347 status.</value>
        [JsonProperty("InsuranceOperation347")]
        public string InsuranceOperation347 { get; set; }

        /// <summary>
        /// Gets or sets the hierarchical deduction status.
        /// </summary>
        /// <value>The hierarchical deduction status.</value>
        [JsonProperty("HierarchicalDeduction")]
        public string HierarchicalDeduction { get; set; }

        /// <summary>
        /// Gets or sets the Shaam group.
        /// </summary>
        /// <value>The Shaam group.</value>
        [JsonProperty("ShaamGroup")]
        public string ShaamGroup { get; set; }

        /// <summary>
        /// Gets or sets the withholding tax certified status.
        /// </summary>
        /// <value>The withholding tax certified status.</value>
        [JsonProperty("WithholdingTaxCertified")]
        public string WithholdingTaxCertified { get; set; }

        /// <summary>
        /// Gets or sets the bookkeeping certified status.
        /// </summary>
        /// <value>The bookkeeping certified status.</value>
        [JsonProperty("BookkeepingCertified")]
        public string BookkeepingCertified { get; set; }

        /// <summary>
        /// Gets or sets the affiliate status.
        /// </summary>
        /// <value>The affiliate status.</value>
        [JsonProperty("Affiliate")]
        public string Affiliate { get; set; }

        /// <summary>
        /// Gets or sets the Datev first data entry status.
        /// </summary>
        /// <value>The Datev first data entry status.</value>
        [JsonProperty("DatevFirstDataEntry")]
        public string DatevFirstDataEntry { get; set; }

        /// <summary>
        /// Gets or sets the use shipped goods account status.
        /// </summary>
        /// <value>The use shipped goods account status.</value>
        [JsonProperty("UseShippedGoodsAccount")]
        public string UseShippedGoodsAccount { get; set; }

        /// <summary>
        /// Gets or sets the house bank IBAN.
        /// </summary>
        /// <value>The house bank IBAN.</value>
        [JsonProperty("HouseBankIBAN")]
        public string HouseBankIBAN { get; set; }

        /// <summary>
        /// Gets or sets the automatic posting status.
        /// </summary>
        /// <value>The automatic posting status.</value>
        [JsonProperty("AutomaticPosting")]
        public string AutomaticPosting { get; set; }

        /// <summary>
        /// Gets or sets the alias name.
        /// </summary>
        /// <value>The alias name.</value>
        [JsonProperty("AliasName")]
        public string AliasName { get; set; }

        /// <summary>
        /// Gets or sets the effective discount.
        /// </summary>
        /// <value>The effective discount.</value>
        [JsonProperty("EffectiveDiscount")]
        public string EffectiveDiscount { get; set; }

        /// <summary>
        /// Gets or sets the no discounts status.
        /// </summary>
        /// <value>The no discounts status.</value>
        [JsonProperty("NoDiscounts")]
        public string NoDiscounts { get; set; }

        /// <summary>
        /// Gets or sets the effective price.
        /// </summary>
        /// <value>The effective price.</value>
        [JsonProperty("EffectivePrice")]
        public string EffectivePrice { get; set; }

        /// <summary>
        /// Gets or sets whether the effective price considers price before discount.
        /// </summary>
        /// <value>Whether the effective price considers price before discount.</value>
        [JsonProperty("EffectivePriceConsidersPriceBeforeDiscount")]
        public string EffectivePriceConsidersPriceBeforeDiscount { get; set; }

        /// <summary>
        /// Gets or sets the residence number.
        /// </summary>
        /// <value>The residence number.</value>
        [JsonProperty("ResidenNumber")]
        public string ResidenNumber { get; set; }

        /// <summary>
        /// Gets or sets the endorsable checks from business partner status.
        /// </summary>
        /// <value>The endorsable checks from business partner status.</value>
        [JsonProperty("EndorsableChecksFromBP")]
        public string EndorsableChecksFromBP { get; set; }

        /// <summary>
        /// Gets or sets the accepts endorsed checks status.
        /// </summary>
        /// <value>The accepts endorsed checks status.</value>
        [JsonProperty("AcceptsEndorsedChecks")]
        public string AcceptsEndorsedChecks { get; set; }

        /// <summary>
        /// Gets or sets the block sending marketing content status.
        /// </summary>
        /// <value>The block sending marketing content status.</value>
        [JsonProperty("BlockSendingMarketingContent")]
        public string BlockSendingMarketingContent { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        /// <value>The update date.</value>
        [JsonProperty("UpdateDate")]
        public string UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the update time.
        /// </summary>
        /// <value>The update time.</value>
        [JsonProperty("UpdateTime")]
        public string UpdateTime { get; set; }

        /// <summary>
        /// Gets or sets the exemption max amount validation type.
        /// </summary>
        /// <value>The exemption max amount validation type.</value>
        [JsonProperty("ExemptionMaxAmountValidationType")]
        public string ExemptionMaxAmountValidationType { get; set; }

        /// <summary>
        /// Gets or sets whether to use the bill-to address to determine tax.
        /// </summary>
        /// <value>Whether to use the bill-to address to determine tax.</value>
        [JsonProperty("UseBillToAddrToDetermineTax")]
        public string UseBillToAddrToDetermineTax { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        /// <value>The create date.</value>
        [JsonProperty("CreateDate")]
        public string CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        /// <value>The create time.</value>
        [JsonProperty("CreateTime")]
        public string CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the FCE relevant status.
        /// </summary>
        /// <value>The FCE relevant status.</value>
        [JsonProperty("FCERelevant")]
        public string FCERelevant { get; set; }

        /// <summary>
        /// Gets or sets the FCE validate base delivery status.
        /// </summary>
        /// <value>The FCE validate base delivery status.</value>
        [JsonProperty("FCEValidateBaseDelivery")]
        public string FCEValidateBaseDelivery { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate for incoming payment status.
        /// </summary>
        /// <value>The exchange rate for incoming payment status.</value>
        [JsonProperty("ExchangeRateForIncomingPayment")]
        public string ExchangeRateForIncomingPayment { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate for outgoing payment status.
        /// </summary>
        /// <value>The exchange rate for outgoing payment status.</value>
        [JsonProperty("ExchangeRateForOutgoingPayment")]
        public string ExchangeRateForOutgoingPayment { get; set; }

        /// <summary>
        /// Gets or sets the FCE as payment means status.
        /// </summary>
        /// <value>The FCE as payment means status.</value>
        [JsonProperty("FCEAsPaymentMeans")]
        public string FCEAsPaymentMeans { get; set; }

        /// <summary>
        /// Gets or sets the WayToPay33.
        /// </summary>
        /// <value>The WayToPay33.</value>
        [JsonProperty("U_BXP_FORMAPAGO33")]
        public string WayToPay33 { get; set; }

        /// <summary>
        /// Gets or sets the PayMethod33.
        /// </summary>
        /// <value>The PayMethod33.</value>
        [JsonProperty("U_BXP_METPAGO33")]
        public string PayMethod33 { get; set; }

        /// <summary>
        /// Gets or sets the Sai.
        /// </summary>
        /// <value>The Sai.</value>
        [JsonProperty("U_SAI")]
        public string Sai { get; set; }

        /// <summary>
        /// Gets or sets the Advisor.
        /// </summary>
        /// <value>The Advisor.</value>
        [JsonProperty("U_ASESOR")]
        public string Advisor { get; set; }

        /// <summary>
        /// Gets or sets the Zone.
        /// </summary>
        /// <value>The Zone.</value>
        [JsonProperty("U_ZONA")]
        public string Zone { get; set; }

        /// <summary>
        /// Gets or sets the Zone2.
        /// </summary>
        /// <value>The Zone2.</value>
        [JsonProperty("U_ZONA2")]
        public string Zone2 { get; set; }

        /// <summary>
        /// Gets or sets the Alias.
        /// </summary>
        /// <value>The Alias.</value>
        [JsonProperty("U_Alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the BirthDate.
        /// </summary>
        /// <value>The BirthDate.</value>
        [JsonProperty("U_Fecha_Nacimiento")]
        public string BirthDate { get; set; }

        /// <summary>
        /// Gets or sets Addresses.
        /// </summary>
        /// <value>The Addresses.</value>
        [JsonProperty("BPAddresses")]
        public List<DoctorAddressDto> Addresses { get; set; }

        /// <summary>
        /// Gets or sets ContactEmployees.
        /// </summary>
        /// <value>The ContactEmployees.</value>
        [JsonProperty("ContactEmployees")]
        public List<DoctorEmployeeDto> ContactEmployees { get; set; }

        /// <summary>
        /// Gets or sets ElectronicProtocols.
        /// </summary>
        /// <value>The ElectronicProtocols.</value>
        [JsonProperty("ElectronicProtocols")]
        public List<DoctorElectronicProtocolDto> ElectronicProtocols { get; set; }

        /// <summary>
        /// Gets or sets PaymentMethods.
        /// </summary>
        /// <value>The PaymentMethods.</value>
        [JsonProperty("BPPaymentMethods")]
        public List<DoctorPaymentMethodDto> PaymentMethods { get; set; }

        /// <summary>
        /// Gets or sets MainUsage.
        /// </summary>
        /// <value>The MainUsage.</value>
        [JsonProperty("U_B1SYS_MainUsage")]
        public string MainUsage { get; set; }

        /// <summary>
        /// Gets or sets BxpRegimen.
        /// </summary>
        /// <value>The BxpRegimen.</value>
        [JsonProperty("U_BXP_REGIMENF")]
        public string BxpRegimen { get; set; }

        /// <summary>
        /// Gets or sets BxpUsoCFDI.
        /// </summary>
        /// <value>The BxpUsoCFDI.</value>
        [JsonProperty("U_BXP_USOCFDI")]
        public string BxpUsoCFDI { get; set; }

        /// <summary>
        /// Gets or sets BxpAddenda.
        /// </summary>
        /// <value>The BxpAddenda.</value>
        [JsonProperty("U_BXP_ADDENDA")]
        public string BxpAddenda { get; set; }

        /// <summary>
        /// Gets or sets BxpDesglosaIEPS.
        /// </summary>
        /// <value>The BxpDesglosaIEPS.</value>
        [JsonProperty("U_BXP_DESGLOSAIEPS")]
        public string BxpDesglosaIEPS { get; set; }

        /// <summary>
        /// Gets or sets BxpNumRegIdTrib.
        /// </summary>
        /// <value>The BxpNumRegIdTrib.</value>
        [JsonProperty("U_BXP_NUMREGIDTRIB")]
        public string BxpNumRegIdTrib { get; set; }

        /// <summary>
        /// Gets or sets BxpNombre.
        /// </summary>
        /// <value>The BxpNombre.</value>
        [JsonProperty("U_BXP_NOMBRE")]
        public string BxpNombre { get; set; }

        /// <summary>
        /// Gets or sets BxpEsfactorante.
        /// </summary>
        /// <value>The BxpEsfactorante.</value>
        [JsonProperty("U_BXP_ESFACTORANTE")]
        public string BxpEsfactorante { get; set; }

        /// <summary>
        /// Gets or sets Properties1.
        /// </summary>
        /// <value>The Properties1.</value>
        [JsonProperty("Properties1")]
        public string Properties1 { get; set; }

        /// <summary>
        /// Gets or sets Properties2.
        /// </summary>
        /// <value>The Properties2.</value>
        [JsonProperty("Properties2")]
        public string Properties2 { get; set; }

        /// <summary>
        /// Gets or sets Properties3.
        /// </summary>
        /// <value>The Properties3.</value>
        [JsonProperty("Properties3")]
        public string Properties3 { get; set; }

        /// <summary>
        /// Gets or sets Properties4.
        /// </summary>
        /// <value>The Properties4.</value>
        [JsonProperty("Properties4")]
        public string Properties4 { get; set; }

        /// <summary>
        /// Gets or sets Properties5.
        /// </summary>
        /// <value>The Properties5.</value>
        [JsonProperty("Properties5")]
        public string Properties5 { get; set; }

        /// <summary>
        /// Gets or sets Properties6.
        /// </summary>
        /// <value>The Properties6.</value>
        [JsonProperty("Properties6")]
        public string Properties6 { get; set; }

        /// <summary>
        /// Gets or sets Properties7.
        /// </summary>
        /// <value>The Properties7.</value>
        [JsonProperty("Properties7")]
        public string Properties7 { get; set; }

        /// <summary>
        /// Gets or sets Properties8.
        /// </summary>
        /// <value>The Properties8.</value>
        [JsonProperty("Properties8")]
        public string Properties8 { get; set; }

        /// <summary>
        /// Gets or sets Properties9.
        /// </summary>
        /// <value>The Properties9.</value>
        [JsonProperty("Properties9")]
        public string Properties9 { get; set; }

        /// <summary>
        /// Gets or sets Properties10.
        /// </summary>
        /// <value>The Properties10.</value>
        [JsonProperty("Properties10")]
        public string Properties10 { get; set; }

        /// <summary>
        /// Gets or sets Properties11.
        /// </summary>
        /// <value>The Properties11.</value>
        [JsonProperty("Properties11")]
        public string Properties11 { get; set; }

        /// <summary>
        /// Gets or sets Properties12.
        /// </summary>
        /// <value>The Properties12.</value>
        [JsonProperty("Properties12")]
        public string Properties12 { get; set; }

        /// <summary>
        /// Gets or sets Properties13.
        /// </summary>
        /// <value>The Properties13.</value>
        [JsonProperty("Properties13")]
        public string Properties13 { get; set; }

        /// <summary>
        /// Gets or sets Properties14.
        /// </summary>
        /// <value>The Properties14.</value>
        [JsonProperty("Properties14")]
        public string Properties14 { get; set; }

        /// <summary>
        /// Gets or sets Properties15.
        /// </summary>
        /// <value>The Properties15.</value>
        [JsonProperty("Properties15")]
        public string Properties15 { get; set; }

        /// <summary>
        /// Gets or sets Properties16.
        /// </summary>
        /// <value>The Properties16.</value>
        [JsonProperty("Properties16")]
        public string Properties16 { get; set; }

        /// <summary>
        /// Gets or sets Properties17.
        /// </summary>
        /// <value>The Properties17.</value>
        [JsonProperty("Properties17")]
        public string Properties17 { get; set; }

        /// <summary>
        /// Gets or sets Properties18.
        /// </summary>
        /// <value>The Properties18.</value>
        [JsonProperty("Properties18")]
        public string Properties18 { get; set; }

        /// <summary>
        /// Gets or sets Properties19.
        /// </summary>
        /// <value>The Properties19.</value>
        [JsonProperty("Properties19")]
        public string Properties19 { get; set; }

        /// <summary>
        /// Gets or sets Properties20.
        /// </summary>
        /// <value>The Properties20.</value>
        [JsonProperty("Properties20")]
        public string Properties20 { get; set; }

        /// <summary>
        /// Gets or sets Properties21.
        /// </summary>
        /// <value>The Properties21.</value>
        [JsonProperty("Properties21")]
        public string Properties21 { get; set; }

        /// <summary>
        /// Gets or sets Properties22.
        /// </summary>
        /// <value>The Properties22.</value>
        [JsonProperty("Properties22")]
        public string Properties22 { get; set; }

        /// <summary>
        /// Gets or sets Properties23.
        /// </summary>
        /// <value>The Properties23.</value>
        [JsonProperty("Properties23")]
        public string Properties23 { get; set; }

        /// <summary>
        /// Gets or sets Properties24.
        /// </summary>
        /// <value>The Properties24.</value>
        [JsonProperty("Properties24")]
        public string Properties24 { get; set; }

        /// <summary>
        /// Gets or sets Properties25.
        /// </summary>
        /// <value>The Properties25.</value>
        [JsonProperty("Properties25")]
        public string Properties25 { get; set; }

        /// <summary>
        /// Gets or sets Properties26.
        /// </summary>
        /// <value>The Properties26.</value>
        [JsonProperty("Properties26")]
        public string Properties26 { get; set; }

        /// <summary>
        /// Gets or sets Properties27.
        /// </summary>
        /// <value>The Properties27.</value>
        [JsonProperty("Properties27")]
        public string Properties27 { get; set; }

        /// <summary>
        /// Gets or sets Properties28.
        /// </summary>
        /// <value>The Properties28.</value>
        [JsonProperty("Properties28")]
        public string Properties28 { get; set; }

        /// <summary>
        /// Gets or sets Properties29.
        /// </summary>
        /// <value>The Properties29.</value>
        [JsonProperty("Properties29")]
        public string Properties29 { get; set; }

        /// <summary>
        /// Gets or sets Properties30.
        /// </summary>
        /// <value>The Properties30.</value>
        [JsonProperty("Properties30")]
        public string Properties30 { get; set; }

        /// <summary>
        /// Gets or sets Properties31.
        /// </summary>
        /// <value>The Properties31.</value>
        [JsonProperty("Properties31")]
        public string Properties31 { get; set; }

        /// <summary>
        /// Gets or sets Properties32.
        /// </summary>
        /// <value>The Properties32.</value>
        [JsonProperty("Properties32")]
        public string Properties32 { get; set; }

        /// <summary>
        /// Gets or sets Properties33.
        /// </summary>
        /// <value>The Properties33.</value>
        [JsonProperty("Properties33")]
        public string Properties33 { get; set; }

        /// <summary>
        /// Gets or sets Properties34.
        /// </summary>
        /// <value>The Properties34.</value>
        [JsonProperty("Properties34")]
        public string Properties34 { get; set; }

        /// <summary>
        /// Gets or sets Properties35.
        /// </summary>
        /// <value>The Properties35.</value>
        [JsonProperty("Properties35")]
        public string Properties35 { get; set; }

        /// <summary>
        /// Gets or sets Properties36.
        /// </summary>
        /// <value>The Properties36.</value>
        [JsonProperty("Properties36")]
        public string Properties36 { get; set; }

        /// <summary>
        /// Gets or sets Properties37.
        /// </summary>
        /// <value>The Properties37.</value>
        [JsonProperty("Properties37")]
        public string Properties37 { get; set; }

        /// <summary>
        /// Gets or sets Properties38.
        /// </summary>
        /// <value>The Properties38.</value>
        [JsonProperty("Properties38")]
        public string Properties38 { get; set; }

        /// <summary>
        /// Gets or sets Properties39.
        /// </summary>
        /// <value>The Properties39.</value>
        [JsonProperty("Properties39")]
        public string Properties39 { get; set; }

        /// <summary>
        /// Gets or sets Properties40.
        /// </summary>
        /// <value>The Properties40.</value>
        [JsonProperty("Properties40")]
        public string Properties40 { get; set; }

        /// <summary>
        /// Gets or sets Properties41.
        /// </summary>
        /// <value>The Properties41.</value>
        [JsonProperty("Properties41")]
        public string Properties41 { get; set; }

        /// <summary>
        /// Gets or sets Properties42.
        /// </summary>
        /// <value>The Properties42.</value>
        [JsonProperty("Properties42")]
        public string Properties42 { get; set; }

        /// <summary>
        /// Gets or sets Properties43.
        /// </summary>
        /// <value>The Properties43.</value>
        [JsonProperty("Properties43")]
        public string Properties43 { get; set; }

        /// <summary>
        /// Gets or sets Properties44.
        /// </summary>
        /// <value>The Properties44.</value>
        [JsonProperty("Properties44")]
        public string Properties44 { get; set; }

        /// <summary>
        /// Gets or sets Properties45.
        /// </summary>
        /// <value>The Properties45.</value>
        [JsonProperty("Properties45")]
        public string Properties45 { get; set; }

        /// <summary>
        /// Gets or sets Properties46.
        /// </summary>
        /// <value>The Properties46.</value>
        [JsonProperty("Properties46")]
        public string Properties46 { get; set; }

        /// <summary>
        /// Gets or sets Properties47.
        /// </summary>
        /// <value>The Properties47.</value>
        [JsonProperty("Properties47")]
        public string Properties47 { get; set; }

        /// <summary>
        /// Gets or sets Properties48.
        /// </summary>
        /// <value>The Properties48.</value>
        [JsonProperty("Properties48")]
        public string Properties48 { get; set; }

        /// <summary>
        /// Gets or sets Properties49.
        /// </summary>
        /// <value>The Properties49.</value>
        [JsonProperty("Properties49")]
        public string Properties49 { get; set; }

        /// <summary>
        /// Gets or sets Properties50.
        /// </summary>
        /// <value>The Properties50.</value>
        [JsonProperty("Properties50")]
        public string Properties50 { get; set; }

        /// <summary>
        /// Gets or sets Properties51.
        /// </summary>
        /// <value>The Properties51.</value>
        [JsonProperty("Properties51")]
        public string Properties51 { get; set; }

        /// <summary>
        /// Gets or sets Properties52.
        /// </summary>
        /// <value>The Properties52.</value>
        [JsonProperty("Properties52")]
        public string Properties52 { get; set; }

        /// <summary>
        /// Gets or sets Properties53.
        /// </summary>
        /// <value>The Properties53.</value>
        [JsonProperty("Properties53")]
        public string Properties53 { get; set; }

        /// <summary>
        /// Gets or sets Properties54.
        /// </summary>
        /// <value>The Properties54.</value>
        [JsonProperty("Properties54")]
        public string Properties54 { get; set; }

        /// <summary>
        /// Gets or sets Properties55.
        /// </summary>
        /// <value>The Properties55.</value>
        [JsonProperty("Properties55")]
        public string Properties55 { get; set; }

        /// <summary>
        /// Gets or sets Properties56.
        /// </summary>
        /// <value>The Properties56.</value>
        [JsonProperty("Properties56")]
        public string Properties56 { get; set; }

        /// <summary>
        /// Gets or sets Properties57.
        /// </summary>
        /// <value>The Properties57.</value>
        [JsonProperty("Properties57")]
        public string Properties57 { get; set; }

        /// <summary>
        /// Gets or sets Properties58.
        /// </summary>
        /// <value>The Properties58.</value>
        [JsonProperty("Properties58")]
        public string Properties58 { get; set; }

        /// <summary>
        /// Gets or sets Properties59.
        /// </summary>
        /// <value>The Properties59.</value>
        [JsonProperty("Properties59")]
        public string Properties59 { get; set; }

        /// <summary>
        /// Gets or sets Properties60.
        /// </summary>
        /// <value>The Properties60.</value>
        [JsonProperty("Properties60")]
        public string Properties60 { get; set; }

        /// <summary>
        /// Gets or sets Properties61.
        /// </summary>
        /// <value>The Properties61.</value>
        [JsonProperty("Properties61")]
        public string Properties61 { get; set; }

        /// <summary>
        /// Gets or sets Properties62.
        /// </summary>
        /// <value>The Properties62.</value>
        [JsonProperty("Properties62")]
        public string Properties62 { get; set; }

        /// <summary>
        /// Gets or sets Properties63.
        /// </summary>
        /// <value>The Properties63.</value>
        [JsonProperty("Properties63")]
        public string Properties63 { get; set; }

        /// <summary>
        /// Gets or sets Properties64.
        /// </summary>
        /// <value>The Properties64.</value>
        [JsonProperty("Properties64")]
        public string Properties64 { get; set; }

        /// <summary>
        /// Gets or sets AdditionalData.
        /// </summary>
        /// <value>The AdditionalData.</value>
        [Newtonsoft.Json.JsonExtensionData]
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}