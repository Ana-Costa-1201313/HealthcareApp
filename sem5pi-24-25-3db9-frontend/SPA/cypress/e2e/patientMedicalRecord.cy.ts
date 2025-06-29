describe('Patient Medical Records Page Test', () => {
  before(() => {
    cy.visit('/login');
    cy.get('#form3Example3').type('admin@hotmail.com');
    cy.get('#form3Example4').type('AAAAAAAAAAA1!');
    cy.get('#loginButton').click();

    cy.get('.main-title').should('be.visible');
  });

  it('Visits the Patient Medical Records page', () => {
    cy.visit('/patientMedicalRecord');
    cy.contains('Patient Medical Records');
    cy.get('.p-datatable-striped tbody')
      .find('tr')
      .its('length')
      .should('be.gte', 1); // Verifica que há pelo menos 1 registro na tabela
  });
});

const randomPatientId = `aaa${Date.now()}@gmail.com`;
const randomAllergies = ['Wheat Allergy', 'Sulfa Drugs Allergy'];
const randomConditions = ['Cholera', 'Generalized anxiety disorder'];
const randomFamilyHistory = ['Heart Disease'];
const randomFreeText = ['Note about patient'];

describe('Patient Medical Records Tests', () => {
  beforeEach(() => {
    cy.visit('/login');
    cy.get('#form3Example3').type('admin@hotmail.com');
    cy.get('#form3Example4').type('AAAAAAAAAAA1!');
    cy.get('#loginButton').click();

    cy.get('.main-title').should('contain.text', 'Healthcare Application');
    cy.visit('/patientMedicalRecord');
    cy.contains('Patient Medical Records');
  });

  it('Create a valid Patient Medical Record', () => {

    cy.visit('/patient');

    cy.get('#buttonCreatePatient').click();

    cy.get('#firstName').type('Kerem');
    cy.get('#lastName').type('Akturkoglu');
    cy.get('#fullName').type('Kerem Akturkoglu');
    cy.get('#gender').type('Masculine');
    cy.get('#dateOfBirth').type('2003-01-01');
    cy.get('#email').type(randomPatientId);
    cy.get('#phone').type('911111111');
    cy.get('#emergencyContact').type('955644633');

    cy.get('#buttonSubmit').click({ force: true });


    cy.visit('/patientMedicalRecord');

    cy.get('#buttonCreateMedicalRecord').click();

    cy.get('#patientEmail').click();
    cy.get('.p-dropdown-item').contains(randomPatientId).click();

    cy.get('#createAllergies').click();
    randomAllergies.forEach((allergy) => {
      cy.get('.p-multiselect-item').contains(allergy).click();
    });
    cy.get('#createAllergies').click();

    cy.get('#createMedicalConditions').click();
    randomConditions.forEach((condition) => {
      cy.get('.p-multiselect-item').contains(condition).click();
    });
    cy.get('#createMedicalConditions').click();


    cy.get('#buttonAddFamilyHistory').click();
    cy.get('#createFamilyHistory_0').type(randomFamilyHistory[0]);


    cy.get('#buttonAddFreeText').click();
    cy.get('#createFreeText_0').type(randomFreeText[0]);

    cy.get('#buttonCreateSubmit').click();

    cy.get('#message').should(($div) => {
      const text = $div.text();
      expect(`SuccessRecord created successfully!`).equal(text);
    });


    cy.visit('/patient');

    cy.get('input[aria-label="Filter Name"]').type("Kerem");

    cy.get('.p-datatable-striped tbody')
      .contains('tr', "Kerem")
      .within(() => {
        cy.get('#buttonDeletePatient').click();
      });

    cy.get('#confirmDelete').click();
  });

  it('Edit Allergies of an Existing Record', () => {
    cy.get('.p-datatable-striped tbody')
      .contains('tr', 'kevinDeBruyne@gmail.com')
      .within(() => {
        cy.get('#buttonEditAllergies').click();
      });

    cy.get('#editAllergies').click();
    cy.get('.p-multiselect-item').contains('Milk Allergy').click();
    cy.get('#editAllergies').click();

    cy.get('#buttonSaveAllergies').click();

    cy.get('#message').should(($div) => {
      const text = $div.text();
      expect('SuccessAllergies updated successfully!').equal(text);
    });
  });

  it('Edit Medical Conditions of an Existing Record', () => {
    cy.get('.p-datatable-striped tbody')
      .contains('tr', "kevinDeBruyne@gmail.com")
      .within(() => {
        cy.get('#buttonEditConditions').click();
      });

    cy.get('#editMedicalConditions').click();
    cy.get('.p-multiselect-item').contains('Obesity').click();
    cy.get('#editMedicalConditions').click();

    cy.get('#buttonSaveConditions').click();

    cy.get('#message').should(($div) => {
      const text = $div.text();
      expect('SuccessMedical conditions updated successfully!').equal(text);
    });
  });

  it('Add Free Text of an Existing Record', () => {
    cy.get('.p-datatable-striped tbody')
      .contains('tr', 'kevinDeBruyne@gmail.com')
      .within(() => {
        cy.get('#buttonEditFreeText').click();
      });

    cy.intercept('PUT', '/api/patientMedicalRecord/*/free-text').as('createFreeTextEntry');

    cy.get('p-dialog').should('be.visible');

    cy.get('#addFreeText').click();
    cy.wait(100);

    cy.get('[id^="freeTextId"]')
      .eq(2)
      .type('eats too much');

    cy.get('#buttonSaveFreeText').click();

    cy.get('#message').should('be.visible').and('contain', 'Success');
    cy.wait('@createFreeTextEntry').then((interception) => {
      expect(interception.response?.statusCode).to.eq(200);
    });
  });


  it('Edit Free Text of an Existing Record', () => {
    cy.get('.p-datatable-striped tbody')
      .contains('tr', 'kevinDeBruyne@gmail.com')
      .within(() => {
        cy.get('#buttonEditFreeText').click();
      });

    cy.intercept('PUT', '/api/patientMedicalRecord/*/free-text').as('createFreeTextEntry');

    cy.get('p-dialog').should('be.visible');
    cy.wait(100);


    cy.get('[id^="freeTextId"]')
      .eq(2)
      .clear()
      .type('eats too much bananas');
    cy.get('#buttonSaveFreeText').click();

    cy.get('#message').should('be.visible').and('contain', 'Success');
    cy.wait('@createFreeTextEntry').then((interception) => {
      expect(interception.response?.statusCode).to.eq(200);
    });
  });


  it('Delete Free Text of an Existing Record', () => {
    cy.get('.p-datatable-striped tbody')
      .contains('tr', 'kevinDeBruyne@gmail.com')
      .within(() => {
        cy.get('#buttonEditFreeText').click();
      });

    cy.intercept('PUT', '/api/patientMedicalRecord/*/free-text').as('createFreeTextEntry');


    cy.get('p-dialog').should('be.visible');
    cy.wait(100);


    cy.get('[id^="deletefreeText"]')
      .eq(2)
      .click();

    cy.get('#buttonSaveFreeText').click();

    cy.get('#message').should('be.visible').and('contain', 'Success');
    cy.wait('@createFreeTextEntry').then((interception) => {
      expect(interception.response?.statusCode).to.eq(200);
    });
  });

  it('Add Family History of an Existing Record', () => {
    cy.get('.p-datatable-striped tbody')
      .contains('tr', 'kevinDeBruyne@gmail.com')
      .within(() => {
        cy.get('#buttonEditFamilyHistory').click();
      });

    cy.intercept('PUT', '/api/patientMedicalRecord/*/family-history').as('createFamilyHistory');

    cy.get('p-dialog').should('be.visible');

    cy.get('#addFamilyHistory').click();
    cy.wait(100);

    cy.get('[id^="familyHistoryId"]')
      .eq(2)
      .type('hearth attack');

    cy.get('#buttonSaveFamilyHistory').click();

    cy.get('#message').should('be.visible').and('contain', 'Success');
    cy.wait('@createFamilyHistory').then((interception) => {
      expect(interception.response?.statusCode).to.eq(200);
    });
  });


  it('Edit Family History of an Existing Record', () => {
    cy.get('.p-datatable-striped tbody')
      .contains('tr', 'kevinDeBruyne@gmail.com')
      .within(() => {
        cy.get('#buttonEditFamilyHistory').click();
      });

    cy.intercept('PUT', '/api/patientMedicalRecord/*/family-history').as('createFamilyHistory');

    cy.get('p-dialog').should('be.visible');
    cy.wait(100);


    cy.get('[id^="familyHistoryId"]')
      .eq(2)
      .clear()
      .type('aneurism');
    cy.get('#buttonSaveFamilyHistory').click();

    cy.get('#message').should('be.visible').and('contain', 'Success');
    cy.wait('@createFamilyHistory').then((interception) => {
      expect(interception.response?.statusCode).to.eq(200);
    });
  });


  it('Delete Family History of an Existing Record', () => {
    cy.get('.p-datatable-striped tbody')
      .contains('tr', 'kevinDeBruyne@gmail.com')
      .within(() => {
        cy.get('#buttonEditFamilyHistory').click();
      });

    cy.intercept('PUT', '/api/patientMedicalRecord/*/family-history').as('createFamilyHistory');


    cy.get('p-dialog').should('be.visible');
    cy.wait(100);


    cy.get('[id^="deletefamilyHistory"]')
      .eq(2)
      .click();

    cy.get('#buttonSaveFamilyHistory').click();

    cy.get('#message').should('be.visible').and('contain', 'Success');
    cy.wait('@createFamilyHistory').then((interception) => {
      expect(interception.response?.statusCode).to.eq(200);
    });
  });

});
