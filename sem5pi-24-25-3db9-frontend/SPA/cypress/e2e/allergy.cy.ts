describe('Allergy Page Test', () => {
  before(() => {
    cy.visit('/login');
    cy.get('#form3Example3').type('admin@hotmail.com');
    cy.get('#form3Example4').type('AAAAAAAAAAA1!');
    cy.get('#loginButton').click();

    cy.get('.main-title').should('be.visible');
  });

  it('Visits the allergy page', () => {
    cy.visit('/allergies');
    cy.contains('Allergy');
    cy.get('.p-datatable-striped tbody')
      .find('tr')
      .its('length')
      .should('be.gte', 2);
  });
});

const randomName = `AllergyName_${Date.now()}`;
const randomCode = `AllergyCode_${Date.now()}`;
const randomNameNew = `AllergyNameNew_${Date.now()}`;

describe('Allergy Test', () => {
  beforeEach(() => {
    cy.visit('/login');
    cy.get('#form3Example3').type('admin@hotmail.com');
    cy.get('#form3Example4').type('AAAAAAAAAAA1!');
    cy.get('#loginButton').click();

    cy.get('.main-title').should('be.visible');
  });

  it('Create allergy', () => {
    cy.visit('/allergies');

    cy.intercept('POST', '/api/allergies').as('createAllergy');

    cy.get('#buttonCreate').click();

    cy.get('#name').type(randomName);
    cy.get('#code').type(randomCode);
    cy.get('#description').type('TestDescription');

    cy.get('#buttonSubmit').find('button').click({ force: true });

    cy.wait('@createAllergy').then((interception) => {
      expect(interception.response?.statusCode).to.eq(201);
    });

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('Success!Your Allergy was added with success').equal(text);
    });
  });

  it('Throw error creating allergy with used name', () => {
    cy.visit('/allergies');

    cy.intercept('POST', '/api/allergies').as('createAllergy');

    cy.get('#buttonCreate').click();

    cy.get('#name').type(randomName);
    cy.get('#code').type(randomCode);
    cy.get('#description').type('TestDescription');

    cy.get('#buttonSubmit').find('button').click({ force: true });

    cy.get('#buttonCreate').click();

    cy.get('#name').type(randomName);
    cy.get('#code').type(randomCode);
    cy.get('#description').type('TestDescription');

    cy.get('#buttonSubmit').find('button').click({ force: true });

    cy.wait('@createAllergy').then((interception) => {
      expect(interception.response?.statusCode).to.eq(400);
    });

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('Failure!Error: This allergy name is already in use!').equal(text);
    });
  });

  it('Throw error creating allergy with used code', () => {
    cy.visit('/allergies');

    cy.intercept('POST', '/api/allergies').as('createAllergy');

    cy.get('#buttonCreate').click();

    cy.get('#name').type(randomName);
    cy.get('#code').type(randomCode);
    cy.get('#description').type('TestDescription');

    cy.get('#buttonSubmit').find('button').click({ force: true });

    cy.get('#buttonCreate').click();

    cy.get('#name').type(randomNameNew);
    cy.get('#code').type(randomCode);
    cy.get('#description').type('TestDescription');

    cy.get('#buttonSubmit').find('button').click({ force: true });

    cy.wait('@createAllergy').then((interception) => {
      expect(interception.response?.statusCode).to.eq(400);
    });

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('Failure!Error: This allergy code is already in use!').equal(text);
    });
  });

  it('Edit allergy', () => {
    cy.visit('/allergies');

    cy.get('input[aria-label="Filter Name"]').type(randomName + '{enter}');

    cy.get('.p-datatable-striped tbody')
      .contains('tr', randomName)
      .within(() => {
        cy.get('#buttonEdit').click();
      });

    cy.get('#name').clear();
    cy.get('#name').type(randomNameNew);

    cy.get('#buttonSubmit').find('button').click({ force: true });

    cy.get('#message').should(($div) => {
      const text = $div.text();

      expect('Success!Your Allergy was edited with success').equal(text);
    });
  });

  it('Get allergy details', () => {
    cy.visit('/allergies');

    cy.get('input[aria-label="Filter Name"]').type(randomNameNew + '{enter}');

    cy.get('.p-datatable-striped tbody')
      .contains('tr', randomNameNew)
      .within(() => {
        cy.get('#buttonDetails').click();
      });

    cy.get('#name').should('contain.text', randomNameNew);
    cy.get('#code').should('contain.text', randomCode);
    cy.get('#description').should('contain.text', 'TestDescription');
  });
});
