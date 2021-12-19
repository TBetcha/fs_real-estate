-- I dropped this and remade it, no data was in so no harm had to change column types
CREATE TABLE IF NOT EXISTS users (
  id UUID NOT NULL,
  username VARCHAR(255) NOT NULL,
  password VARCHAR(255) NOT NULL,
  first_name VARCHAR(255),
  last_name VARCHAR(255),

  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS houses (
  id UUID NOT NULL,
  user_id UUID NOT NULL,
  bedroom INT NOT NULL,
  bathroom FLOAT NOT NULL,
  square_feet INT NOT NULL,
  street_number VARCHAR(255) NOT NULL,
  city VARCHAR(255) NOT NULL,
  state VARCHAR(36) NOT NULL,
  zip INT NOT NULL,

  CONSTRAINT FK_houses_users FOREIGN KEY(user_id) REFERENCES users(id),
  PRIMARY KEY (id)
);