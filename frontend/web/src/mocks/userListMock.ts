import { Clasification, ClasificationsResponse } from 'src/app/model/http/users';

export const UserListMock = {
    code: 200,
    comments: 20,
    exceptionMessage: null,
    success: true,
    userError: null,
    response: [
        {
            id: 'cda94a8a-366e-46c9-b120-68c6edce3c44',
            userName: 'quimico2',
            firstName: 'quimico',
            lastName: 'dos',
            role: 2,
            password: 'QXhpdHkyMDIw',
            activo: 1,
            piezas: 100,
            asignable: 1,
            deleted: false,
            classification: 'MG',
            tecnicId: null,
            technicalRequire: false,
            classificationDescription: 'Magistral (MG)',
            isChecked: true
        },
        {
            id: '6758b8d8-af3a-4c77-9dc5-73a1ba72e190',
            userName: 'Logistica1',
            firstName: 'Ricardo',
            lastName: 'Logistica',
            role: 3,
            password: 'QXhpdHkyMDI0',
            activo: 1,
            piezas: 200,
            asignable: 1,
            deleted: false,
            classification: 'BE, DZ, MG',
            tecnicId: null,
            technicalRequire: false,
            classificationDescription: 'Bioequal (BE), Dermazone (DZ), Magistral (MG)',
            isChecked: false
        },
        {
            id: '6758b8d8-af3a-4c77-9dc5-73a1ba72e190',
            userName: 'Diseño1',
            firstName: 'Ricardo',
            lastName: 'Diseño',
            role: 4,
            password: 'QXhpdHkyMDI0',
            activo: 1,
            piezas: 200,
            asignable: 1,
            deleted: false,
            classification: 'BE, DZ, MG',
            tecnicId: null,
            technicalRequire: false,
            classificationDescription: 'Todas',
            isChecked: true
        },
        {
            id: '6758b8d8-af3a-4c77-9dc5-73a1ba72e190',
            userName: 'Admin1',
            firstName: 'Ricardo',
            lastName: 'Admin',
            role: 1,
            password: 'QXhpdHkyMDI0',
            activo: 1,
            piezas: 200,
            asignable: 1,
            deleted: false,
            classification: 'BE, DZ, MG',
            tecnicId: null,
            technicalRequire: false,
            classificationDescription: 'Bioequal (BE), Dermazone (DZ), Magistral (MG)',
            isChecked: false
        }
    ]
};

export const userClasificationMock: ClasificationsResponse = {
    code: 200,
    userError: null,
    exceptionMessage: null,
    success: true,
    response: [
        {
            value: 'Todas',
            description: 'TODAS',
            classificationQfb: false
        },
        {
            value: 'MN',
            description: 'OMIGENOMICS',
            classificationQfb: false
        },
        {
            value: 'BE',
            description: 'BIOELITE',
            classificationQfb: false
        },
        {
            value: 'MN',
            description: 'Bioelite (MN)',
            classificationQfb: true
        },
        {
            value: 'BE',
            description: 'Bioequal (BE)',
            classificationQfb: true
        },
        {
            value: 'DZ',
            description: 'Dermazone (DZ)',
            classificationQfb: true
        },
        {
            value: 'MG',
            description: 'Magistral (MG)',
            classificationQfb: true
        },
        {
            value: 'MG',
            description: 'MAGISTRALES',
            classificationQfb: false
        }
    ]
};
