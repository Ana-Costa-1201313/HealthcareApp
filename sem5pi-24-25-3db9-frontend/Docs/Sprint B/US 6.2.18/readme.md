# US 6.2.18

## 1. Context

This task appears in the middle of the project's development, to be able to create a new operation type.


## 2. Requirements

**US 6.2.18** As an Admin, I want to add new types of operations, so that I can reflect the available medical procedures in the system. 

**Acceptance Criteria:**

- Admins can add new operation types with attributes like: 
- Operation Name 
- Required Staff by Specialization 
- Estimated Duration 
- The system validates that the operation name is unique. 
- The system logs the creation of new operation types and makes them available for scheduling immediately. 


**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.
The backend component of the US should already be developed.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

```
Q: The document you provided divides surgical times into "specific phases of the surgery," whereas the main statement only mentions recording the total surgery time. Should the system, therefore, store and specify the time spent on each phase of the surgery, or is it sufficient to only record the total surgery time without detailing the time distribution across each phase?

A: when describing an operation type, the system must record the time for each phase
```

```
Q: Are the different phases of surgery relevant to the business and should they be recorded in the system?

A: yes. they are important due to the time it takes each phase and in the future for the planning of different teams (e.g., cleaning team)
```

```
Q: In the document with the surgeries, they all have 3 phases and respective duration:

-Anesthesia/patient preparation

-Surgery

-Cleaning

Can we assume while creating a new operation type, that the surgery must always have this 3 phases?

A: Yes
```


The following **HTTP requests** will be implemented:
- POST (to create a new operation type)


## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Sequence Diagram (Level 1)

![SSD_Lvl1.png](SD1.svg) 


### 4.2. Sequence Diagram (Level 2)

![SSD_Lvl1.png](SD2.svg) 


### 4.3. Sequence Diagram (Level 3)

![SSD_Lvl1.png](SD3.svg) 


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
- the created operation type must have the same data as the input.
- an error message should be displayed when the input is invalid or mandatory.

Unitary tests to the component will be implemented.

E2E Tests will also check all the US flow, from the user interaction to the backend server.

## 5. Implementation

The implementation of this US is according to the design, as can be seen in the SD and CD presented before.

All commits referred the corresponding issue in GitHub, using the #41 tag, as well as a relevant commit message.

It was implemented in the branch feature/6.2.18-frontend-admin-add-operation-types.

The estimated duration for this US is 15h.


## 6. Integration/Demonstration

To create a new Operation Type, run the Backoffice, Auth and SPA app then go to the Operation Type page and click on the "+" icon to add a operation type.

## 7. Observations

This work was guided by the project provided in ARQSI classes.