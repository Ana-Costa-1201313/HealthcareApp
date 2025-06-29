describe('Patient Page Test', () => {
  before(() => {
    cy.visit('/login');
    cy.get('#form3Example3').type('admin@hotmail.com');
    cy.get('#form3Example4').type('AAAAAAAAAAA1!');
    cy.get('#loginButton').click();

    cy.get('.main-title').should('be.visible');
  });

  it('Visits the patient page', () => {
    cy.visit('/patient');
    cy.contains('Patient');
    cy.get('.p-datatable-striped tbody')
      .find('tr')
      .its('length')
      .should('be.gte', 2);
  });
});

describe('Patient Test', () => {
  beforeEach(() => {
    cy.visit('/login');
    cy.get('#form3Example3').type('admin@hotmail.com');
    cy.get('#form3Example4').type('AAAAAAAAAAA1!');
    cy.get('#loginButton').click();

    cy.get('.main-title').should('be.visible');
  });

  it('Create patient', () => {
    cy.visit('/patient');

    cy.get('#buttonCreatePatient').click();

    cy.get('#firstName').type('Kerem');
    cy.get('#lastName').type('Akturkoglu');
    cy.get('#fullName').type('Kerem Akturkoglu');
    cy.get('#gender').type('Masculine');
    cy.get('#dateOfBirth').type('2003-01-01');
    cy.get('#email').type('kAktur@gmail.com');
    cy.get('#phone').type('909897965');
    cy.get('#emergencyContact').type('955444333');

    cy.get('#buttonSubmit').click({ force: true });

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('SuccessKerem Akturkoglu\'s profile has been created successfully.').equal(text);
    });
  });

  it('Get Patient Details Test', () => {
    cy.visit('/patient');

    cy.get('input[aria-label="Filter Name"]').type('Kerem' + '{enter}');

    cy.get('.p-datatable-striped tbody')
      .contains('tr', 'Kerem')
      .within(() => {
        cy.get('#buttonDetailPatient').click();
      });

    cy.get('#name').should('contain.text', 'Kerem');
    cy.get('#email').should('contain.text', 'kAktur@gmail.com');
    cy.get('#phone').should('contain.text', '909897965');
  });

  it('Update Patient Test', () => {
    cy.visit('/patient');

    cy.get('input[aria-label="Filter Name"]').type('Kerem' + '{enter}');

    cy.get('.p-datatable-striped tbody')
      .contains('tr', 'Kerem')
      .within(() => {
        cy.get('#buttonEditPatient').click();
      });
    cy.get('#firstName').clear().type("Tomas");
    cy.get('#lastName').clear().type("Araujo");
    cy.get('#fullName').clear().type("Tomas Araujo");

    cy.get('#buttonSubmit').click({ force: true });

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('SuccessTomas Araujo\'s profile has been edited successfully.').equal(text);
    });
  });

  it('Delete Patient Test', () => {
    cy.visit('/patient');

    cy.get('input[aria-label="Filter Name"]').type("Tomas");

    cy.get('.p-datatable-striped tbody')
      .contains('tr', "Tomas")
      .within(() => {
        cy.get('#buttonDeletePatient').click();
      });

    cy.get('#confirmDelete').click();

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('SuccessTomas Araujo\'s profile has been deleted successfully.').equal(
        text
      );
    });
  });

});