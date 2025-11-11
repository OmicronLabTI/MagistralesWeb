/**
 * ============================================================================
 * MODULE: AutomaticBillingRoutingModule
 * ============================================================================
 * Purpose:
 *   Defines the routing configuration for the "Automatic Billing" feature
 *   module within the Magistrales Web application.
 *
 * Functional Context (HU OM-7160):
 *   This routing module enables navigation to the "Automatic Billing Dashboard",
 *   which displays the historical records of invoices created automatically
 *   from the warehouse system.
 *
 * Responsibilities:
 *   • Registers the route for the Automatic Billing page.
 *   • Ensures that when the path is accessed, the associated component
 *     (`AutomaticBillingComponent`) is rendered.
 *   • Supports Angular's lazy-loading structure by encapsulating the route
 *     within a feature-specific routing module.
 *
 * Usage:
 *   This module is imported by `AutomaticBillingModule`.
 *   The router will load this component when navigating to:
 *     /automatic-billing
 * ============================================================================
 */

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AutomaticBillingComponent } from './automatic-billing.component';

/**
 * ---------------------------------------------------------------------------
 * Route Configuration
 * ---------------------------------------------------------------------------
 *  path: '' → Base route for this feature module.
 *  component: AutomaticBillingComponent → Displays the billing dashboard view.
 * ---------------------------------------------------------------------------
 */
const routes: Routes = [
  { path: '', component: AutomaticBillingComponent }
];

/**
 * ---------------------------------------------------------------------------
 * AutomaticBillingRoutingModule
 * ---------------------------------------------------------------------------
 *  - Registers the route definitions above.
 *  - Exports RouterModule so that child components in this module can
 *    participate in the application's routing system.
 * ---------------------------------------------------------------------------
 */
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AutomaticBillingRoutingModule { }
