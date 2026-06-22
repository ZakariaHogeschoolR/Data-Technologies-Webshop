CREATE SCHEMA pii;

CREATE TABLE pii.users_pii
(
    user_id  INT PRIMARY KEY REFERENCES public.users (id) ON DELETE CASCADE,
    email    TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    address  TEXT,
    postcode TEXT
);

INSERT INTO pii.users_pii (user_id, email, password, address, postcode)
SELECT id, email, password, address, postcode
FROM public.users;

ALTER TABLE public.users
    DROP COLUMN email;
ALTER TABLE public.users
    DROP COLUMN password;
ALTER TABLE public.users
    DROP COLUMN address;
ALTER TABLE public.users
    DROP COLUMN postcode;

CREATE ROLE webshop_catalog NOLOGIN;
CREATE ROLE webshop_transact NOLOGIN;
CREATE ROLE webshop_manage NOLOGIN;
CREATE ROLE webshop_dba NOLOGIN;

CREATE ROLE webshop_app LOGIN;
CREATE ROLE webshop_admin LOGIN;

GRANT webshop_catalog, webshop_transact TO webshop_app;
GRANT webshop_catalog, webshop_transact, webshop_manage TO webshop_admin;

REVOKE CREATE ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA pii FROM PUBLIC;

GRANT USAGE ON SCHEMA public TO webshop_catalog, webshop_transact, webshop_manage, webshop_dba;
GRANT USAGE ON SCHEMA pii TO webshop_manage, webshop_dba;

REVOKE ALL ON public.users FROM PUBLIC;
GRANT SELECT ON public.users TO webshop_transact;
GRANT SELECT, INSERT, UPDATE, DELETE ON public.users TO webshop_manage;
GRANT ALL ON public.users TO webshop_dba;

REVOKE ALL ON pii.users_pii FROM PUBLIC;
GRANT SELECT, INSERT, UPDATE ON pii.users_pii TO webshop_manage;
GRANT ALL ON pii.users_pii TO webshop_dba;

REVOKE ALL ON public.products, public.teams, public.category, public.product_categories FROM PUBLIC;
GRANT SELECT ON public.products, public.teams, public.category, public.product_categories TO webshop_catalog;
GRANT ALL ON public.products, public.teams, public.category, public.product_categories TO webshop_manage;
GRANT ALL ON public.products, public.teams, public.category, public.product_categories TO webshop_dba;

REVOKE ALL ON public.winkelwagen, public.winkelwagen_users, public.orders, public.wishlist FROM PUBLIC;
GRANT SELECT, INSERT, UPDATE, DELETE ON public.winkelwagen, public.winkelwagen_users, public.orders, public.wishlist TO webshop_transact;
GRANT ALL ON public.winkelwagen, public.winkelwagen_users, public.orders, public.wishlist TO webshop_manage;
GRANT ALL ON public.winkelwagen, public.winkelwagen_users, public.orders, public.wishlist TO webshop_dba;

REVOKE ALL ON public."Payments" FROM PUBLIC;
GRANT SELECT, INSERT ON public."Payments" TO webshop_transact;
GRANT ALL ON public."Payments" TO webshop_manage;
GRANT ALL ON public."Payments" TO webshop_dba;

GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO webshop_transact, webshop_manage, webshop_dba;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA pii TO webshop_manage, webshop_dba;

GRANT SELECT, INSERT ON public.order_items TO webshop_transact;
GRANT ALL ON public.order_items TO webshop_manage;
GRANT ALL ON public.order_items TO webshop_dba;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO webshop_transact;