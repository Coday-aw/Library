"use client";
import BookForm from "./components/BookForm";
import Modal from "./components/Modal";
import { useState } from "react";
import Navbar from "./components/Navbar";
function page() {
  const [Open, setOpen] = useState(false);

  const CloseModal = () => {
    setOpen(false);
  };
  const OpenModal = () => {
    setOpen(true);
  };
  return (
    <div className="">
      <Navbar OpenModal={OpenModal} />

      {Open && (
        <Modal CloseModal={CloseModal}>
          <BookForm CloseModal={CloseModal} />
        </Modal>
      )}
    </div>
  );
}
export default page;
