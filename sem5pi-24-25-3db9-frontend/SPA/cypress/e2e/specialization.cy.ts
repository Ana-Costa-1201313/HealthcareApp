describe('Specialization Page Test', () => {
    before(() => {
        cy.visit('/login');
        cy.get('#form3Example3').type('admin@hotmail.com');
        cy.get('#form3Example4').type('AAAAAAAAAAA1!');
        cy.get('#loginButton').click();

        cy.get('.main-title').should('be.visible');
    });

    it('Visits the specialization page', () => {
        cy.visit('/specialization');
        cy.contains('Specialization');
    });
});

const randomName = `Specialization_${Date.now()}`;

describe('Specialization Tests', () => {
    beforeEach(() => {
        cy.visit('/login');
        cy.get('#form3Example3').type('admin@hotmail.com');
        cy.get('#form3Example4').type('AAAAAAAAAAA1!');
        cy.get('#loginButton').click();

        cy.get('.main-title').should('be.visible');
    });

    it('Create new Specialization', () => {
        cy.visit('/specialization');

        cy.get('#buttonCreateSpecialization').click();

        cy.get('#name').type(randomName);
        cy.get('#description').type('description');

        cy.get('#buttonCreateSubmit').find('button').click({ force: true });

        cy.get('#message').should(($div) => {
            const text = $div.text();

            expect('Success!Your Specialization was added with success').equal(text);
        });
    });

    it('Create duplicate Specialization', () => {
        cy.visit('/specialization');

        cy.get('#buttonCreateSpecialization').click();

        cy.get('#name').type(randomName);
        cy.get('#description').type('description');

        cy.get('#buttonCreateSubmit').find('button').click({ force: true });

        cy.get('#message').should(($div) => {
            const text = $div.text();

            expect('Failure!Error: This specialization name is already being used.').equal(text);
        });
    });

    it('Edit an existing Specialization', () => {
        const updatedName = `Updated_Specialization`;
        const updatedDescription = 'Updated description';

        cy.visit('/specialization');

        cy.get('#buttonEditSpec').first().click();

        cy.get('p-dialog').should('be.visible');

        cy.get('#name').clear().type(updatedName);
        cy.get('#description').clear().type(updatedDescription);

        cy.get('.p-button-success').click();

        cy.get('#message').should('be.visible').and(($div) => {
            const text = $div.text();
            expect(text).to.equal('Success!Updated_Specialization\'s has been edited successfully.');
        });

    });

    it('Get specialization details', () => {
        cy.visit('/specialization');
    
        cy.get('input[aria-label="Filter Name"]').type(randomName + '{enter}');
    
        cy.get('.p-datatable-striped tbody')
          .contains('tr', randomName)
          .within(() => {
            cy.get('#buttonDetails').click();
          });
    
        cy.get('#name').should('contain.text', randomName);
        cy.get('#description').should('contain.text', 'description');
      });
});

