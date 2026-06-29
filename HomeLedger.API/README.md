1 - setup Logger -
a - setup built-in logger - setup custom logger + Serilog + Seq

2 - Test:
a - Unit testing
b - integration testing
c - test driven development

3 - setup CI/CD Pipeline

4 - rate limitting, caching -> in the future before production, after connecting to frontend

- setup default bills
- monthly income,expense ( data aggregation )

- login:
  secure: false for dev

---

- GET: Family -> done
  - setup invitation - DONE
  - setup sharing invitation link -> user clicks that should redirect user to login/signup page -> invitation page
  - email service

---

Remove FamilyId from Users table

Create another table:
FamilyMembers:
UserId
FamilyId

---

- Authorization: return bills if user is logged in (authenticated)
