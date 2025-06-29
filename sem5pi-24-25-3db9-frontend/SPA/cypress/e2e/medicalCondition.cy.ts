describe('Medical Condition Page Test', () => {
    before(() => {
        cy.visit('/login');
        cy.get('#form3Example3').type('admin@hotmail.com');
        cy.get('#form3Example4').type('AAAAAAAAAAA1!');
        cy.get('#loginButton').click();

        cy.get('.main-title').should('be.visible');
    });

    it('Visits the medical condition page', () => {
        cy.visit('/medicalcondition');
        cy.contains('Medical Condition');
    });
});

const randomName = `MedicalConditionName_${Date.now()}`;
const randomCode = `MedicalConditionCode_${Date.now()}`;

describe('Medical Condition Tests', () => {
    beforeEach(() => {
        cy.visit('/login');
        cy.get('#form3Example3').type('admin@hotmail.com');
        cy.get('#form3Example4').type('AAAAAAAAAAA1!');
        cy.get('#loginButton').click();

        cy.get('.main-title').should('be.visible');
    });

    it('Create valid Medical Condition', () => {
        cy.visit('/medicalcondition');

        cy.intercept('POST', '/api/MedicalConditions').as('createMedicalCondition');

        cy.get('#buttonCreateMedicalCondition').click();

        cy.get('#name').type(randomName);
        cy.get('#code').type(randomCode);
        cy.get('#description').type("description");

        cy.get('#buttonCreateSymptoms').click();
        cy.get('#buttonCreateSymptoms').click();

        cy.get('#symptomText0').clear().type("symptom 1");
        cy.get('#symptomText1').clear().type("symptom 2");

        cy.get('#buttonCreateSubmit').find('button').click({ force: true });

        cy.wait('@createMedicalCondition').then((interception) => {
            expect(interception.response?.statusCode).to.eq(201);
        });

        cy.get('#message').should(($div) => {
            const text = $div.text();

            expect('Success!Your Medical Condition was added with success').equal(text);
        });
    });

    it('Create Medical Condition with duplicated name', () => {
        cy.visit('/medicalcondition');

        cy.intercept('POST', '/api/MedicalConditions').as('createMedicalCondition');

        cy.get('#buttonCreateMedicalCondition').click();

        cy.get('#name').type(randomName);
        cy.get('#code').type(randomCode);
        cy.get('#description').type("description");

        cy.get('#buttonCreateSubmit').find('button').click({ force: true });

        cy.wait('@createMedicalCondition').then((interception) => {
            expect(interception.response?.statusCode).to.eq(400);
        });

        cy.get('#message').should(($div) => {
            const text = $div.text();

            expect('Failure!Error: This medical condition name is already in use!').equal(text);
        });
    });

    it('Create Medical Condition with duplicated code', () => {
        cy.visit('/medicalcondition');

        cy.intercept('POST', '/api/MedicalConditions').as('createMedicalCondition');

        cy.get('#buttonCreateMedicalCondition').click();

        cy.get('#name').type(randomName + "2");
        cy.get('#code').type(randomCode);
        cy.get('#description').type("description");

        cy.get('#buttonCreateSubmit').find('button').click({ force: true });

        cy.wait('@createMedicalCondition').then((interception) => {
            expect(interception.response?.statusCode).to.eq(400);
        });

        cy.get('#message').should(($div) => {
            const text = $div.text();

            expect('Failure!Error: This medical condition code is already in use!').equal(text);
        });
    });

    it('Get Operation Type Details Test', () => {
        cy.visit('/medicalcondition');

        cy.get('input[aria-label="Filter Name"]').type(randomName + '{enter}');

        cy.get('.p-datatable-striped tbody')
            .contains('tr', randomName)
            .within(() => {
                cy.get('#buttonDetailMedicalCondition').click();
            });

        cy.get('#name').should('contain.text', randomName);
        cy.get('#code').should('contain.text', randomCode);
        cy.get('#description').should('contain.text', 'description');
    });

    it('Update Medical Condition Test', () => {
        cy.visit('/medicalcondition');

        cy.intercept('PUT', '/api/MedicalConditions/*').as('updateMedicalCondition');
    
        cy.get('input[aria-label="Filter Name"]').type(randomName + '{enter}');
    
        cy.get('.p-datatable-striped tbody')
          .contains('tr', randomName)
          .within(() => {
            cy.get('#buttonUpdateMedicalCondition').click();
          });
        cy.get('#name').clear().type(randomName + "123");
        cy.get('#description').clear().type("desc");
    
        cy.get('#buttonUpdateSubmit').find('button').click({ force: true });
        
        cy.wait('@updateMedicalCondition').then((interception) => {
            expect(interception.response?.statusCode).to.eq(200);
        });
        cy.get('#message').should(($div) => {
          const text = $div.text();
    
          expect('Success!Medical Condition updated successfully!').equal(text);
        });
      });


});

