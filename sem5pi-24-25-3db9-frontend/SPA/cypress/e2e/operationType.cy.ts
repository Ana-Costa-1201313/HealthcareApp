describe('Operation Type Page Test', () => {
  before(() => {
    cy.visit('/login');
    cy.get('#form3Example3').type('admin@hotmail.com');
    cy.get('#form3Example4').type('AAAAAAAAAAA1!');
    cy.get('#loginButton').click();

    cy.get('.main-title').should('be.visible');
  });

  it('Visits the operation type page', () => {
    cy.visit('/operationtype');
    cy.contains('Operation Type');
    cy.get('.p-datatable-striped tbody')
      .find('tr')
      .its('length')
      .should('be.gte', 2);
  });
});

const randomName = `OpTypeName_${Date.now()}`;

describe('Operation Type Tests', () => {
  beforeEach(() => {
    cy.visit('/login');
    cy.get('#form3Example3').type('admin@hotmail.com');
    cy.get('#form3Example4').type('AAAAAAAAAAA1!');
    cy.get('#loginButton').click();

    cy.get('.main-title').should('be.visible');
  });
  
  it('Create Operation Type', () => {
    cy.visit('/operationtype');

    cy.get('#buttonCreateOperationType').click();

    cy.get('#name').type(randomName);
    cy.get('#preparation').type('20');
    cy.get('#surgery').type('50');
    cy.get('#cleaning').type('25');

    cy.get('#buttonCreateRequiredStaff').click();
    cy.get('#buttonCreateRequiredStaff').click();

    cy.get('#total0').clear().type('2');
    cy.get('#total1').clear().type('5');

    cy.get('#specialization0').click();
    cy.get('.p-dropdown-item').contains('Dermatology').click();

    cy.get('#specialization1').click();
    cy.get('.p-dropdown-item').contains('Nurse Anesthesia').click();

    cy.get('#buttonCreateSubmit').find('button').click({ force: true });

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('Success!Your Operation Type was added with success').equal(text);
    });
  });

  it('Get Operation Type Details Test', () => {
    cy.visit('/operationtype');

    cy.get('input[aria-label="Filter Name"]').type(randomName + '{enter}');

    cy.get('.p-datatable-striped tbody')
      .contains('tr', randomName)
      .within(() => {
        cy.get('#buttonDetailOperationType').click();
      });

    cy.get('#name').should('contain.text', randomName);
    cy.get('#preparation').should('contain.text', '20');
    cy.get('#surgery').should('contain.text', '50');
    cy.get('#cleaning').should('contain.text', '25');
  });

  it('Update Operation Type Test', () => {
    cy.visit('/operationtype');

    cy.get('input[aria-label="Filter Name"]').type(randomName + '{enter}');

    cy.get('.p-datatable-striped tbody')
      .contains('tr', randomName)
      .within(() => {
        cy.get('#buttonUpdateOperationType').click();
      });
    cy.get('#name').clear().type(randomName + "123");
    cy.get('#preparation').clear().type("10");
    cy.get('#surgery').clear().type("100");
    cy.get('#cleaning').clear().type("20");

    cy.get('#buttonUpdateSubmit').find('button').click({ force: true });

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('Success!Operation Type updated successfully!').equal(text);
    });
  });

  it('Deactivate Operation Type Test', () => {
    cy.visit('/operationtype');

    cy.get('input[aria-label="Filter Name"]').type(randomName + "123" + '{enter}');

    cy.get('.p-datatable-striped tbody')
      .contains('tr', randomName + "123")
      .within(() => {
        cy.get('#buttonDeactivateOperationType').click();
      });

    cy.get('#buttonConfirmDeactivate').click();

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('Success!The Operation Type "' + randomName + "123" + '" was deactivated with success').equal(
        text
      );
    });
  });
});

