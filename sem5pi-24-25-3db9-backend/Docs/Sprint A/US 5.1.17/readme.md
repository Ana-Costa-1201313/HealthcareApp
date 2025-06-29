# US 5.1.17

## 1. Context

This task appears in the start of the project's development, to be able to update an operation requisition.


## 2. Requirements

**US 5.1.17** As a Doctor, I want to update an operation requisition, so that the Patient has access to the necessary healthcare

**Acceptance Criteria:**

- Doctors can update operation requests they created (e.g., change the deadline or priority).
- The system checks that only the requesting doctor can update the operation request.
- The system logs all updates to the operation request (e.g., changes to priority or deadline).
- Updated requests are reflected immediately in the system and notify the Planning Module of any changes.

**Dependencies/References:**

The user logged in must be registered in the system as a 'Doctor'.

## 3. Analysis

The following requirements specified by the client were considered during the development of this user story:

- **Question:** You want to log all updates to the operation request. Do you plan to have this info available in the app or is this just for audit purposes ?
  - **Answer:** The history of the operation type definition is part of the application's data. If the user needs to view the details of an operation that was performed last year, they need to be able to see the operation configuration that was in place at that time.

- **Question:** There was a previous question, "What information can physicians update on an operating requisition?", with the following answer, "Physicians can update the operating time, priority, and description text, but not change the patient.". However, half of this answer applies to the Operation Type, instead of the Operation Request. Operation Requests have, at least, an ID, a Patient, an Operation Type, a Doctor, a Deadline Date, and a Priority. Considering the previous answer, the doctor cannot change the Patient ID but can change the Priority. Besides the Priority, could the doctor also update the Deadline Date or Operation Type?
  - **Answer:** 
The answer was about operation requests, not operation types. I believe the term "operation time" in the original answer was the reason for this misunderstanding, as it means the expected deadline for the request, not the duration. Thus, the doctor can change the deadline, the priority, and the description. The doctor cannot change the operation type nor the patient.

- **Question:**  So in the draft documentation, it says that some of the information they can update is the operating time and the priority.
I'm wondering if there's any more relevant information that the doctor can access and change.
  - **Answer:** 
For the operating requisition, you can access the requisition itself, the requisition text. [...] So the doctor can change the priority, the operation time and that description text.

## 4. Design



### 4.1. Level 1 Sequence Diagram

![US5.1.17 N1 SD](US5.1.17%20N1%20SD.svg)

### 4.2. Level 2 Sequence Diagram

![US5.1.17 N2 SD](US5.1.17%20N2%20SD.svg)

### 4.3. Level 3 Sequence Diagram

![US5.1.17 N3 SD](US5.1.17%20N3%20SD.svg)

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




## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #21 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To edit an operation request, run the Backoffice app and send a PUT HTTP request with your doctor ID and the updated operation request data.
Then send a GET request with the operation request ID and check the updated data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.