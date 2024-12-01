interface ModalProps {
  children: React.ReactNode;
  CloseModal: () => void;
}

const Modal = ({ children, CloseModal }: ModalProps) => {
  return (
    <div
      className=" fixed inset-0 bg-black opacity-70 flex justify-center items-center"
      arira-modal="true"
      role="true"
      onClick={CloseModal}
    >
      <div onClick={(e) => e.stopPropagation()}>{children}</div>
    </div>
  );
};
export default Modal;
