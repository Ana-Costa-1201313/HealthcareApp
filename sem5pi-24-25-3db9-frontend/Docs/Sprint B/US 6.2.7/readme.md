# US 6.2.7

## 1. Context

This task appears in the middle of the project's development, to be able to edit a patient profile.


## 2. Requirements

**US 6.2.7** As an Admin, I want to edit an existing patient profile, so that I can update their information when needed

**Acceptance Criteria:**

- Admins can search for and select a patient profile to edit.
- Editable fields include name, contact information, medical history, and allergies.
- Changes to sensitive data (e.g., contact information) trigger an email notification to the patient.
- The system logs all profile changes for auditing purposes.

**Dependencies/References:**

It is also required that the user is registered and logged in as an admin and that the patient profile is already in the system.
The backend component of this US must be already developed.

## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

- **

- **Question:** In this US an admin can edit a user profile. Does the system display a list of all users or the admin searchs by ID? Or both?
  - **Answer:** this requirement is for the editing of the user profile. from a usability point of view, the user should be able to start this feature either by searching for a specific user or listing all users and selecting one.
note that we are not doing the user interface of the system in this sprint.


- **Question:** Regarding the editing of patient information, is contact information the only sensitive data? Is it the only data that triggers an email notification?
  - **Answer:** faz parte das vossas responsabilidades no âmbito do módulo de proteçãod e dados e de acordo com a politica que venham a definir



###  Domain Model excerpt
![DM Patient](DM%20Patient.png)

The following **HTTP requests** will be implemented:
- GET (to check available patient)
- PUT (to edit a specific patient)
- PATCH (to edit some data of a specific patient)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US6.2.7 N1 SD](US6.2.7%20N1%20SD.png)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US6.2.7 N2 SD](US6.2.7%20N2%20SD.png)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view.

![US6.2.7 N3 SD](US6.2.7%20N3%20SD.png)





### 4.4. Applied Design Patterns

- **Domain Driven Development (DDD):** the focus is the business logic and not the implementation.
- **Data Transfer Object (DTO):** gives an abstraction layer to the domain, so that it's only presented specific information regarding the object.
- **Model View Controller (MVC):** allows the re-usability of components and promotes a more modular approach to the code, making it easier to manage and maintain.
- **Repository pattern:** allows access to data without sharing the details of data storing, like the database connection.
- **Service pattern:** helps keeping high cohesion and low coupling in the code by separating complex business logic from the rest of the system. They also promote reuse, as multiple parts of the system can use the same service to perform common operations.
- **Test Driven Development (TDD):** planning the tests previously to the code gives orientation lines to the development process.
- **Onion Architecture:** concentric layers structure that puts the Domin Model as the core. Promotes modularity, flexibility and testability.
- **Inversion of Control:** the responsability of object creation and dependency management belongs to a framework or external entity, so that the class doesn't need to. Promotes flexibility and decoupling.
- **Dependency Injection:** used to implement inversion of control. The dependencies are injected into a class from the outside.


### 4.5. Tests

The following tests are to be developed:
- the edited patient profile must have the same data as the input.
- an error message should be displayed when the input is invalid or mandatory.
- the page must reload to show the updated patient profile.
- a success message should be shown after updating.

Unitary tests to the component will be implemented.

E2E Tests will also check all the US flow, from the user interaction to the backend server.

## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #30 tag, as well as a relevant commit message.
The estimated duration for this US is 15h.

## 6. Integration/Demonstration

To edit a patient profile, run the Backoffice, Auth and the SPA app, then go to the Patient page and click on "pencil" icon to edit a patient profile.

## 7. Observations

This work was guided by the project provided in ARQSI classes.