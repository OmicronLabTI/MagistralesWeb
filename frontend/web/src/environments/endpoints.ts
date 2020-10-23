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
    components: `${environment.baseUrl}/api/sapadapter/componentes`,
    updateFormula: `${environment.baseUrl}/api/pedidos/formula`,
    processOrdersDetail: `${environment.baseUrl}/api/pedidos/processByOrder`,
    placeOrdersAutomatic: `${environment.baseUrl}/api/pedidos/asignar/automatico`,
    cancelOrders: `${environment.baseUrl}/api/pedidos/salesOrder/cancel`,
    cancelOrdersDetail: `${environment.baseUrl}/api/pedidos/fabOrder/cancel`,
    finalizeOrders: `${environment.baseUrl}/api/pedidos/salesOrder/finish`,
    finalizeOrdersDetail: `${environment.baseUrl}/api/pedidos/fabOrder/finish`,
    getProducts: `${environment.baseUrl}/api/sapadapter/products`,
    createIsolatedOrder: `${environment.baseUrl}/api/pedidos/fabOrder/isolated`,
    reAssignManual: `${environment.baseUrl}/api/pedidos/reasignar/manual`,
    getNextBatchCode: `${environment.baseUrl}/api/sapadapter/batchcode/next`,
    checkIfExistsBatchCode: `${environment.baseUrl}/api/sapadapter/batchcode/exists`,
    getRecipes: `${environment.baseUrl}/api/sapadapter/recipe`,
},
  inventoryBatches: {
    getInventoryBatches: `${environment.baseUrl}/api/sapadapter/componentes/lotes/`,
    assignBatches: `${environment.baseUrl}/api/pedidos/assignBatches`
  },
  orders: {
    endPointOrders: `${environment.baseUrl}/api/pedidos/fabOrder`,
    saveMyList: `${environment.baseUrl}/api/pedidos/components/custom`,
    createPdf: `${environment.baseUrl}/api/pedidos/print/orders`,
    savedComments: `${environment.baseUrl}/api/pedidos/saleorder/comments`,
    finishLabels: `${environment.baseUrl}/api/pedidos/finish/label`
  },
  productivity: {
    getProductivity: `${environment.baseUrl}/api/pedidos/qfb/productivity`,
    getWorkLoad: `${environment.baseUrl}/api/pedidos/qfb/workload`,
  },
  materialRequest: {
    getPreMaterialRequest: `${environment.baseUrl}/api/warehouses/prerequest/rawmaterial`,
    postMaterialRequest: `${environment.baseUrl}/api/warehouses/request/rawmaterial`
  },
  reporting: {
    getRawMaterialRequestFilePreview: `${environment.baseUrl}/api/reporting/preview/request/rawmaterial/pdf`
  }
};

export const TokenExcludedEndpoints = [
  '/api/oauth/oauthrs'
];
