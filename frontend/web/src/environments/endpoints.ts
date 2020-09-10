import { environment } from './environment';
export const Endpoints = {
  security: {
    login: `${environment.baseUrl}/api/oauth/oauthrs/authorize`,
    refresh: `${environment.baseUrl}/api/oauth/oauthrs/renew`
  },
  users: {
    createUser: `${environment.baseUrl}/api/usuarios/createuser`,
    roles: `${environment.baseUrl}/api/catalogos/getroles`,
    getUsers: `${environment.baseUrl}/api/usuarios/getUsers`,
    delete: `${environment.baseUrl}/api/usuarios/deactivateUser`,
    update: `${environment.baseUrl}/api/usuarios/updateUser`,
    getUser: `${environment.baseUrl}/api/usuarios/user/`,
    qfbs: `${environment.baseUrl}/api/usuarios/role`,
    qfbsWithOrders: `${environment.baseUrl}/api/usuarios/qfb/countOrders`
  },
  pedidos: {
    getPedidos: `${environment.baseUrl}/api/sapadapter/orders`,
    getDetallePedido: `${environment.baseUrl}/api/sapadapter/detail/`,
    processOrders: `${environment.baseUrl}/api/pedidos/processOrders`,
    getFormulaDetail: `${environment.baseUrl}/api/sapadapter/formula`,
    placeOrders: `${environment.baseUrl}/api/pedidos/asignar/manual`,
    getComponents: `${environment.baseUrl}/api/sapadapter/componentes`,
    updateFormula: `${environment.baseUrl}/api/pedidos/formula`,
    processOrdersDetail: `${environment.baseUrl}/api/pedidos/processByOrder`,
    placeOrdersAutomatic: `${environment.baseUrl}/api/pedidos/asignar/automatico`,
    cancelOrders: `${environment.baseUrl}/api/pedidos/salesOrder/cancel`,
    cancelOrdersDetail: `${environment.baseUrl}/api/pedidos/fabOrder/cancel`,
    finalizeOrders: `${environment.baseUrl}/api/pedidos/salesOrder/finish`,
    finalizeOrdersDetail: `${environment.baseUrl}/api/pedidos/fabOrder/finish`,
    getProducts: `${environment.baseUrl}/api/sapadapter/products`,
    createIsolatedOrder: `${environment.baseUrl}/api/pedidos/fabOrder/isolated`,
    reAssignManual: `${environment.baseUrl}/api/pedidos/reasignar/manual`
  },
  inventoryBatches: {
    getInventoryBatches: `${environment.baseUrl}/api/sapadapter/componentes/lotes/`,
    assignBatches: `${environment.baseUrl}/api/pedidos/assignBatches`
  },
  orders: {
    getOrders: `${environment.baseUrl}/api/pedidos/fabOrder`,
    saveMyList: `${environment.baseUrl}/api/pedidos/components/custom`
  },
  productivity: {
    getProductivity: `${environment.baseUrl}/api/pedidos/qfb/productivity`,
  }
};

export const TokenExcludedEndpoints = [
  '/api/oauth/oauthrs'
];
