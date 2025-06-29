describe('Room Type Page Test', () => {
  before(() => {
      cy.visit('/roomtype');
      cy.get('#form3Example3').type('admin@hotmail.com');
      cy.get('#form3Example4').type('AAAAAAAAAAA1!');
      cy.get('#loginButton').click();

      cy.get('.main-title').should('be.visible');
  });

  it('Visits the room Type page', () => {
      cy.visit('/roomtype');
      cy.contains('Room Types');
  });
});

var randoName = `Roomtype_${Date.now()}`;

describe('Room Type Tests', () => {
  beforeEach(() => {
      cy.visit('/login');
      cy.get('#form3Example3').type('admin@hotmail.com');
      cy.get('#form3Example4').type('AAAAAAAAAAA1!');
      cy.get('#loginButton').click();

      cy.get('.main-title').should('be.visible');
  });

  it('Create new Room Type', () => {
      cy.intercept('POST', '/api/SurgeryRoom/createRoomType').as('createRoomType');
      cy.visit('/roomtype');

      cy.get('#buttonCreateRoomType').click();

      cy.get('#surgeryRoomType').type(randoName);
      cy.get('#description').type('description');
      cy.get('#fitForSurgery').type('true');

      cy.get('#buttonSubmit').click({ force: true });

      cy.wait('@createRoomType').then((interception) => {
        // Expect the request to have succeeded
        expect([200, 201]).to.include(interception.response.statusCode);
      });
  });

  it('Create duplicate Room Type', () => {
    cy.intercept('POST', '/api/SurgeryRoom/createRoomType').as('createRoomType');
    cy.visit('/roomtype');

    cy.get('#buttonCreateRoomType').click();

    cy.get('#surgeryRoomType').type(randoName);
    cy.get('#description').type('description');
    cy.get('#fitForSurgery').type('true');

    cy.get('#buttonSubmit').click({ force: true });

    cy.wait('@createRoomType').then((interception) => {
      // Expect the request to fail with a client or server error
      expect([400, 409]).to.include(interception.response.statusCode);
    });
    
  });

});