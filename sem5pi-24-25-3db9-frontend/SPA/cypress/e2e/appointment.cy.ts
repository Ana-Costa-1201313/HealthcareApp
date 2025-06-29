describe('Appointment Page Test', () => {
    before(() => {
        cy.visit('/login');
        cy.get('#form3Example3').type('admin@hotmail.com');
        cy.get('#form3Example4').type('AAAAAAAAAAA1!');
        cy.get('#loginButton').click();

        cy.get('.main-title').should('be.visible');
    });

    it('Visits the appointment page', () => {
        cy.visit('/appointment');
        cy.contains('Appointment');
        cy.get('.p-datatable-striped tbody')
            .find('tr')
            .its('length')
            .should('be.gte', 2);
    });
});

describe('Appointment Tests', () => {
    beforeEach(() => {
        cy.visit('/login');
        cy.get('#form3Example3').type('admin@hotmail.com');
        cy.get('#form3Example4').type('AAAAAAAAAAA1!');
        cy.get('#loginButton').click();

        cy.get('.main-title').should('be.visible');
    });

    it('Create Appointment', () => {
        cy.visit('/appointment');

        cy.intercept('POST', '/api/Appointment').as('createAppointment');

        cy.get('#buttonCreateAppointment').click();

        cy.get('#opRequest1', { timeout: 10000 }).should('be.visible').click();

        cy.get('#appointmentDate').click().type('2024-10-11T00:00:00');

        cy.get('#appointmentRoom').click().type('104');

        cy.get('#buttonCreateSubmit').find('button').click({ force: true });

        cy.wait('@createAppointment').then((interception) => {
            expect(interception.response?.statusCode).to.eq(201);
        });
    });


    it('Update Appointment Test', () => {
        cy.visit('/appointment');

        cy.intercept('PATCH', '/api/Appointment/*').as('updateAppointment');


        cy.get('input[aria-label="Filter Operation Type"]').type('ACL Reconstruction Surgery' + '{enter}');

        cy.get('.p-datatable-striped tbody')
            .contains('tr', 'ACL Reconstruction Surgery')
            .within(() => {
                cy.get('#buttonUpdateAppointment').click();
            });

        cy.get('#surgeryRoomNumber')
            .should('be.visible')
            .wait(500)
            .clear()
            .type("101");

        cy.get('#buttonUpdateSubmit').find('button').click({ force: true });

        cy.wait('@updateAppointment').then((interception) => {
            expect(interception.response?.statusCode).to.eq(200);
        });
    });

    // it('Get Appointment Details Test', () => {
    //     cy.visit('/appointment');

    //     cy.get('input[aria-label="Filter Name"]').type(randomName + '{enter}');

    //     cy.get('.p-datatable-striped tbody')
    //         .contains('tr', randomName)
    //         .within(() => {
    //             cy.get('#buttonDetailappointment').click();
    //         });

    //     cy.get('#name').should('contain.text', randomName);
    //     cy.get('#preparation').should('contain.text', '20');
    //     cy.get('#surgery').should('contain.text', '50');
    //     cy.get('#cleaning').should('contain.text', '25');
    // });


    // it('Deactivate Appointment Test', () => {
    //     cy.visit('/appointment');

    //     cy.get('input[aria-label="Filter Name"]').type(randomName + "123" + '{enter}');

    //     cy.get('.p-datatable-striped tbody')
    //         .contains('tr', randomName + "123")
    //         .within(() => {
    //             cy.get('#buttonDeactivateappointment').click();
    //         });

    //     cy.get('#buttonConfirmDeactivate').click();

    //     cy.get('#message').should(($div) => {
    //         const text = $div.text();

    //         expect('Success!The Appointment "' + randomName + "123" + '" was deactivated with success').equal(
    //             text
    //         );
    //     });
    // });
});