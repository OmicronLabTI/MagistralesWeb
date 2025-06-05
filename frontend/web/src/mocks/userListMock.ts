import { Clasification, ClasificationsResponse } from 'src/app/model/http/users';

export const UserListMock = {
    code: 200,
    comments: 20,
    exceptionMessage: null,
    success: true,
    userError: null,
    response: [
        {
            activo: 0,
            firstName: 'Benigno',
            id: 'cda94a8a-366e-46c9-b120-68c6edce3c44',
            isChecked: false,
            lastName: 'Alvarado',
            role: 1,
            userName: 'beni',
            classification: 'MN',
            tecnicId: null,
            technicalRequire: false,
        },
        {
            activo: 1,
            firstName: 'gabriel',
            id: '00ed2f9f-3eb2-49c4-909c-06af680d1f10',
            isChecked: false,
            lastName: 'cruz',
            role: 2,
            userName: 'gabox',
            classification: 'BE',
            tecnicId: null,
            technicalRequire: false
        },
        {
            activo: 0,
            firstName: 'sergio',
            id: '00ed2f9f-3eb2-49c4-909c-06af68f0d1f10',
            isChecked: false,
            lastName: 'flores',
            role: 2,
            userName: 'serch',
            classification: 'MG',
            tecnicId: null,
            technicalRequire: false
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
